using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using NcpAdminBlazor.Client.Models;
using NcpAdminBlazor.Client.Providers;
using NcpAdminBlazor.Client.Pages.Authentication;
using NcpAdminBlazor.Client.Stores;
using NetCorePal.Extensions.Dto;

namespace NcpAdminBlazor.Client.Services;

public interface ITokenSessionService
{
    Task<TokenStorageSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default);

    Task<string?> EnsureAccessTokenAsync(CancellationToken cancellationToken = default);

    Task SignInAsync(
        string accessToken,
        string refreshToken,
        DateTimeOffset? accessTokenExpiry,
        DateTimeOffset? refreshTokenExpiry,
        string userId,
        CancellationToken cancellationToken = default);

    Task SignOutAsync(
        bool redirectToLogin = true,
        CancellationToken cancellationToken = default);
}

internal sealed class TokenSessionService(
    TokenStore storage,
    System.Net.Http.HttpClient httpClient,
    TokenAuthenticationStateProvider stateProvider,
    NavigationManager navigation,
    ILogger<TokenSessionService> logger) : ITokenSessionService
{
    private static readonly SemaphoreSlim RefreshSemaphore = new(1, 1);

    public Task<TokenStorageSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default) =>
        storage.GetAsync(cancellationToken);

    public async Task<string?> EnsureAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var snapshot = await storage.GetAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(snapshot.AccessToken) && !IsTokenExpiring(snapshot.AccessTokenExpiry))
        {
            return snapshot.AccessToken;
        }

        return await RefreshTokenAsync(cancellationToken);
    }

    public async Task SignInAsync(
        string accessToken,
        string refreshToken,
        DateTimeOffset? accessTokenExpiry,
        DateTimeOffset? refreshTokenExpiry,
        string userId,
        CancellationToken cancellationToken = default)
    {
        await storage.SaveAsync(TokenStorageSnapshot.Create(
            accessToken,
            refreshToken,
            accessTokenExpiry,
            refreshTokenExpiry,
            userId), cancellationToken);

        stateProvider.RaiseAuthenticationStateChanged();
    }

    public async Task SignOutAsync(bool redirectToLogin = true, CancellationToken cancellationToken = default)
    {
        await storage.ClearAsync(cancellationToken);
        if (redirectToLogin)
        {
            navigation.NavigateTo(Login.PageUri);
        }

        stateProvider.RaiseAuthenticationStateChanged();
    }

    private async Task<string?> RefreshTokenAsync(CancellationToken cancellationToken)
    {
        await RefreshSemaphore.WaitAsync(cancellationToken);
        try
        {
            var latestSnapshot = await storage.GetAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(latestSnapshot.AccessToken) &&
                !IsTokenExpiring(latestSnapshot.AccessTokenExpiry))
            {
                return latestSnapshot.AccessToken;
            }

            if (string.IsNullOrWhiteSpace(latestSnapshot.RefreshToken) ||
                string.IsNullOrWhiteSpace(latestSnapshot.UserId))
            {
                logger.LogInformation("刷新令牌缺失，执行登出流程。");
                await SignOutAsync(cancellationToken: cancellationToken);
                return null;
            }

            if (IsTokenExpired(latestSnapshot.RefreshTokenExpiry))
            {
                logger.LogInformation("刷新令牌已过期，执行登出流程。");
                await SignOutAsync(cancellationToken: cancellationToken);
                return null;
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/refresh-token")
                {
                    Content = new FormUrlEncodedContent(new Dictionary<string, string?>
                    {
                        { "refreshToken", latestSnapshot.RefreshToken },
                        { "userId", latestSnapshot.UserId }
                    })
                };
                var response = await httpClient.SendAsync(request, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning("刷新令牌请求失败，状态码: {StatusCode}，执行登出流程。", response.StatusCode);
                    await SignOutAsync(cancellationToken: cancellationToken);
                    return null;
                }

                var result =
                    await response.Content.ReadFromJsonAsync<ResponseData<NcpAdminBlazorWebAspNetCoreMyTokenResponse>>(
                        cancellationToken: cancellationToken);
                if (result is
                    {
                        Success: true, Data:
                        {
                            AccessToken: { } newToken,
                            RefreshToken: { } newRefreshToken,
                            AccessTokenExpiry: { } accessTokenExpiry,
                            RefreshTokenExpiry: { } refreshTokenExpiry,
                        } data
                    })
                {
                    await SignInAsync(
                        newToken,
                        newRefreshToken,
                        accessTokenExpiry,
                        refreshTokenExpiry,
                        data.UserId ?? latestSnapshot.UserId ?? string.Empty,
                        cancellationToken);

                    return newToken;
                }
                else
                {
                    logger.LogWarning("刷新令牌响应无效，执行登出流程。");
                    await SignOutAsync(cancellationToken: cancellationToken);
                    return null;
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "刷新令牌过程中发生异常，执行登出流程。");
                await SignOutAsync(cancellationToken: cancellationToken);
                return null;
            }
        }
        finally
        {
            RefreshSemaphore.Release();
        }
    }

    private static bool IsTokenExpiring(DateTimeOffset? expiry)
    {
        if (!expiry.HasValue)
        {
            return true;
        }

        return expiry <= DateTimeOffset.UtcNow.AddMinutes(1);
    }

    private static bool IsTokenExpired(DateTimeOffset? expiry) => !expiry.HasValue || expiry <= DateTimeOffset.UtcNow;
}