using Blazored.LocalStorage;
using NcpAdminBlazor.Client.HttpClient.Auth;
using NcpAdminBlazor.Client.Models;

namespace NcpAdminBlazor.Client.Services;

public sealed class LocalStorageAuthTokenService(
    ILocalStorageService localStorage,
    IServiceProvider serviceProvider,
    ILogger<LocalStorageAuthTokenService> logger)
    : IAuthTokenService
{
    public event EventHandler? TokensCleared;

    private const string SnapshotKey = "auth_token_snapshot";
    private static readonly SemaphoreSlim RefreshSemaphore = new(1, 1);

    private TokenStorageSnapshot _cachedSnapshot = TokenStorageSnapshot.Empty;
    private DateTimeOffset _cacheExpiry = DateTimeOffset.MinValue;

    public async ValueTask<TokenStorageSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        if (_cachedSnapshot != TokenStorageSnapshot.Empty && _cacheExpiry > DateTimeOffset.UtcNow)
        {
            return _cachedSnapshot;
        }

        var snapshot = await localStorage.GetItemAsync<TokenStorageSnapshot?>(SnapshotKey, cancellationToken)
                       ?? TokenStorageSnapshot.Empty;

        _cachedSnapshot = snapshot;
        _cacheExpiry = DateTimeOffset.UtcNow.AddSeconds(30);

        return snapshot;
    }

    public async ValueTask<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var snapshot = await GetSnapshotAsync(cancellationToken);
            if (!snapshot.HasAccessToken)
            {
                await ClearTokensAsync(cancellationToken);
                return null;
            }

            if (!IsExpired(snapshot.AccessTokenExpiry)) return snapshot.AccessToken;
            logger.LogDebug("访问令牌即将过期，尝试续期。");
            if (await TryRefreshTokenAsync(cancellationToken))
            {
                snapshot = await GetSnapshotAsync(cancellationToken);
            }
            else
            {
                await ClearTokensAsync(cancellationToken);
                return null;
            }

            return snapshot.AccessToken;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取访问令牌失败。");
            return null;
        }
    }

    public async ValueTask<string?> GetRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var snapshot = await GetSnapshotAsync(cancellationToken);
            return snapshot.RefreshToken;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "获取刷新令牌失败。");
            return null;
        }
    }

    public async ValueTask SetTokensAsync(TokenStorageSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

        await localStorage.SetItemAsync(SnapshotKey, snapshot, cancellationToken);
        _cachedSnapshot = snapshot;
        _cacheExpiry = DateTimeOffset.UtcNow.AddSeconds(30);

        logger.LogDebug("令牌快照已更新。");
    }

    public async ValueTask ClearTokensAsync(CancellationToken cancellationToken = default)
    {
        await localStorage.RemoveItemAsync(SnapshotKey, cancellationToken);
        _cachedSnapshot = TokenStorageSnapshot.Empty;
        _cacheExpiry = DateTimeOffset.MinValue;

        logger.LogDebug("已清除本地令牌快照。");
        TokensCleared?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask<bool> TryRefreshTokenAsync(CancellationToken cancellationToken = default)
    {
        await RefreshSemaphore.WaitAsync(cancellationToken);
        try
        {
            var snapshot = await GetSnapshotAsync(cancellationToken);

            if (!snapshot.HasRefreshToken || string.IsNullOrWhiteSpace(snapshot.UserId))
            {
                logger.LogDebug("缺少刷新令牌或用户标识，无法续期。");
                return false;
            }

            if (IsExpired(snapshot.RefreshTokenExpiry))
            {
                logger.LogInformation("刷新令牌已过期，清除凭据。");
                await ClearTokensAsync(cancellationToken);
                return false;
            }

            try
            {
                var apiClient = serviceProvider.GetRequiredService<ApiClient>();

                var response = await apiClient.Api.User.RefreshToken.PostAsync(new FastEndpointsSecurityTokenRequest
                    {
                        UserId = snapshot.UserId,
                        RefreshToken = snapshot.RefreshToken,
                    },
                    rc => rc.Options.Add(new AllowAnonymousRequestOption()),
                    cancellationToken: cancellationToken);

                if (response is
                    {
                        Success: true,
                        Data:
                        {
                            AccessToken: { } newToken,
                            RefreshToken: { } newRefreshToken,
                            AccessTokenExpiry: { } accessExpiry,
                            RefreshTokenExpiry: { } refreshExpiry,
                        } data
                    })
                {
                    var updated = new TokenStorageSnapshot(
                        newToken,
                        newRefreshToken,
                        accessExpiry,
                        refreshExpiry,
                        data.UserId ?? snapshot.UserId);

                    await SetTokensAsync(updated, cancellationToken);
                    logger.LogInformation("访问令牌已续期。");
                    return true;
                }

                logger.LogWarning("刷新令牌响应无效，清除凭据。");
                await ClearTokensAsync(cancellationToken);
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "刷新访问令牌时发生异常。");
                await ClearTokensAsync(cancellationToken);
                return false;
            }
        }
        finally
        {
            RefreshSemaphore.Release();
        }
    }

    private static bool IsExpired(DateTimeOffset? expiry) => !expiry.HasValue || expiry <= DateTimeOffset.UtcNow;
}

public sealed record TokenStorageSnapshot(
    string? AccessToken,
    string? RefreshToken,
    DateTimeOffset? AccessTokenExpiry,
    DateTimeOffset? RefreshTokenExpiry,
    string? UserId)
{
    public bool HasAccessToken => !string.IsNullOrWhiteSpace(AccessToken);


    public bool HasRefreshToken => !string.IsNullOrWhiteSpace(RefreshToken);

    public static TokenStorageSnapshot Empty { get; } = new(null, null, null, null, null);
}