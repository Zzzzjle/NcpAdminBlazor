using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using NcpAdminBlazor.Client.Extensions;
using NcpAdminBlazor.Client.Stores;

namespace NcpAdminBlazor.Client.Providers
{
    public class TokenAuthenticationStateProvider(TokenStore tokenStorage)
        : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var snapshot = await tokenStorage.GetAsync();
            var token = snapshot.AccessToken;
            var identity = string.IsNullOrWhiteSpace(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(token.ParseClaimsFromJwt(), "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public void RaiseAuthenticationStateChanged() =>
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}