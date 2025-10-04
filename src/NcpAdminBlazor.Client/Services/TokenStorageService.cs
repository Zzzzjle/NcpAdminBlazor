using Blazored.LocalStorage;

namespace NcpAdminBlazor.Client.Services;

public sealed class TokenStorageSnapshot
{
    private TokenStorageSnapshot(
        string? accessToken,
        string? refreshToken,
        DateTimeOffset? accessTokenExpiry,
        DateTimeOffset? refreshTokenExpiry,
        string? userId)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        AccessTokenExpiry = accessTokenExpiry;
        RefreshTokenExpiry = refreshTokenExpiry;
        UserId = userId;
    }

    public string? AccessToken { get; }

    public string? RefreshToken { get; }

    public DateTimeOffset? AccessTokenExpiry { get; }

    public DateTimeOffset? RefreshTokenExpiry { get; }

    public string? UserId { get; }

    public static TokenStorageSnapshot Empty { get; } = new(null, null, null, null, null);

    public static TokenStorageSnapshot Create(
        string? accessToken,
        string? refreshToken,
        DateTimeOffset? accessTokenExpiry,
        DateTimeOffset? refreshTokenExpiry,
        string? userId) =>
        new(accessToken, refreshToken, accessTokenExpiry, refreshTokenExpiry, userId);
}

public interface ITokenStorageService
{
    Task<TokenStorageSnapshot> GetAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(TokenStorageSnapshot snapshot, CancellationToken cancellationToken = default);

    Task ClearAsync(CancellationToken cancellationToken = default);
}

internal class TokenStorageService(ILocalStorageService localStorage) : ITokenStorageService
{
    private const string TokenKey = "token";
    private const string RefreshTokenKey = "refreshToken";
    private const string AccessTokenExpiryKey = "accessTokenExpiry";
    private const string RefreshTokenExpiryKey = "refreshTokenExpiry";
    private const string UserIdKey = "userId";

    public async Task<TokenStorageSnapshot> GetAsync(CancellationToken cancellationToken = default)
    {
        var accessToken = await localStorage.GetItemAsync<string>(TokenKey, cancellationToken);
        var refreshToken = await localStorage.GetItemAsync<string>(RefreshTokenKey, cancellationToken);
        var accessTokenExpiry = await localStorage.GetItemAsync<DateTimeOffset?>(AccessTokenExpiryKey, cancellationToken);
        var refreshTokenExpiry = await localStorage.GetItemAsync<DateTimeOffset?>(RefreshTokenExpiryKey, cancellationToken);
        var userId = await localStorage.GetItemAsync<string>(UserIdKey, cancellationToken);

        if (string.IsNullOrEmpty(accessToken) && string.IsNullOrEmpty(refreshToken) && string.IsNullOrEmpty(userId))
        {
            return TokenStorageSnapshot.Empty;
        }

        return TokenStorageSnapshot.Create(accessToken, refreshToken, accessTokenExpiry, refreshTokenExpiry, userId);
    }

    public async Task SaveAsync(TokenStorageSnapshot snapshot, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(snapshot);

    await localStorage.SetItemAsync(TokenKey, snapshot.AccessToken ?? string.Empty, cancellationToken);
    await localStorage.SetItemAsync(RefreshTokenKey, snapshot.RefreshToken ?? string.Empty, cancellationToken);
    await localStorage.SetItemAsync(AccessTokenExpiryKey, snapshot.AccessTokenExpiry, cancellationToken);
    await localStorage.SetItemAsync(RefreshTokenExpiryKey, snapshot.RefreshTokenExpiry, cancellationToken);
    await localStorage.SetItemAsync(UserIdKey, snapshot.UserId ?? string.Empty, cancellationToken);
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await localStorage.RemoveItemAsync(TokenKey, cancellationToken);
        await localStorage.RemoveItemAsync(RefreshTokenKey, cancellationToken);
        await localStorage.RemoveItemAsync(AccessTokenExpiryKey, cancellationToken);
        await localStorage.RemoveItemAsync(RefreshTokenExpiryKey, cancellationToken);
        await localStorage.RemoveItemAsync(UserIdKey, cancellationToken);
    }
}
