using Blazored.LocalStorage;
using Microsoft.Kiota.Abstractions.Authentication;

namespace NcpAdminBlazor.Client.HttpClient.Auth;

public class AccessTokenProvider(ILocalStorageService localStorage) : IAccessTokenProvider
{
    private static readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<string> GetAuthorizationTokenAsync(Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        var token = await localStorage.GetItemAsync<string>("token", cancellationToken);
        var accessTokenExpire = await localStorage.GetItemAsync<DateTime?>("accessTokenExpire", cancellationToken);
        var refreshToken = await localStorage.GetItemAsync<string>("refreshToken", cancellationToken);
        var refreshTokenExpire = await localStorage.GetItemAsync<DateTime?>("refreshTokenExpire", cancellationToken);
        if (accessTokenExpire.HasValue && accessTokenExpire <= DateTimeOffset.UtcNow.AddMinutes(1))
        {
            await _semaphore.WaitAsync(cancellationToken);
            try
            {
                accessTokenExpire = await localStorage.GetItemAsync<DateTime?>("accessTokenExpire", cancellationToken);
                if (accessTokenExpire.HasValue && accessTokenExpire <= DateTime.UtcNow.AddMinutes(1))
                {
                    await localStorage.RemoveItemAsync("token", cancellationToken);
                    await localStorage.RemoveItemAsync("accessTokenExpire", cancellationToken);
                    token = null;
                }

            }
        }

        return token ?? string.Empty;
    }

    public AllowedHostsValidator AllowedHostsValidator { get; } = new();
}