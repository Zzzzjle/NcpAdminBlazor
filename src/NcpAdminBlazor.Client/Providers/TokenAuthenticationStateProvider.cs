using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using NcpAdminBlazor.Client.Extensions;

namespace NcpAdminBlazor.Client.Providers
{
    public class TokenAuthenticationStateProvider(ILocalStorageService localStorage)
        : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await localStorage.GetItemAsync<string>("token");
            var identity = string.IsNullOrWhiteSpace(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(token.ParseClaimsFromJwt(), "jwt");

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task Login(string token,
            string refreshToken,
            DateTimeOffset? accessTokenExpiry,
            DateTimeOffset? refreshTokenExpiry)
        {
            await localStorage.SetItemAsync("token", token);
            await localStorage.SetItemAsync("refreshToken", refreshToken);
            await localStorage.SetItemAsync("accessTokenExpiry", accessTokenExpiry);
            await localStorage.SetItemAsync("refreshTokenExpiry", refreshTokenExpiry);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task Logout()
        {
            await localStorage.RemoveItemAsync("token");
            await localStorage.RemoveItemAsync("refreshToken");
            await localStorage.RemoveItemAsync("accessTokenExpiry");
            await localStorage.RemoveItemAsync("refreshTokenExpiry");

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}