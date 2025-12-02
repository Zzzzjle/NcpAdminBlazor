using Microsoft.Kiota.Abstractions.Authentication;

namespace NcpAdminBlazor.Client.Kiota;

public sealed class AccessTokenProvider(IAuthTokenService tokenService) : IAccessTokenProvider
{
    public AllowedHostsValidator AllowedHostsValidator { get; } = new();

    public async Task<string> GetAuthorizationTokenAsync(
        Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        var token = await tokenService.GetTokenAsync(cancellationToken);
        return token ?? string.Empty;
    }
}