using Microsoft.Kiota.Abstractions.Authentication;
using NcpAdminBlazor.Client.Services;

namespace NcpAdminBlazor.Client.HttpClient.Auth;

public class AccessTokenProvider(ITokenSessionService tokenSessionService) : IAccessTokenProvider
{
    public AllowedHostsValidator AllowedHostsValidator { get; } = new();

    public async Task<string> GetAuthorizationTokenAsync(
        Uri uri,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        var token = await tokenSessionService.EnsureAccessTokenAsync(cancellationToken);
        return token ?? string.Empty;
    }
}