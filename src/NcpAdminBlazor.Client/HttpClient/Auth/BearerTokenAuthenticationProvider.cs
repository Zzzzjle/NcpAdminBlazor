using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace NcpAdminBlazor.Client.HttpClient.Auth;

public class BearerTokenAuthenticationProvider(IAuthenticationProvider authenticationProvider)
    : IAuthenticationProvider
{
    public new async Task AuthenticateRequestAsync(
        RequestInformation request,
        Dictionary<string, object>? additionalAuthenticationContext = null,
        CancellationToken cancellationToken = default)
    {
        if (request.RequestOptions.Any(x => x is AllowAnonymousRequestOption))
        {
            // Skip authentication for requests that allow anonymous access
            return;
        }

        await authenticationProvider.AuthenticateRequestAsync(request, additionalAuthenticationContext,
            cancellationToken);
    }
}