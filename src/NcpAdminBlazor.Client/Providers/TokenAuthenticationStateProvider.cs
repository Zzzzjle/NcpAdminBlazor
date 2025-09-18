using System.Net.Http.Headers;
using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using NcpAdminBlazor.Client.Extensions;

namespace NcpAdminBlazor.Client.Providers
{
    public class TokenAuthenticationStateProvider(HttpClient httpClient, ILocalStorageService localStorage)
        : AuthenticationStateProvider
    {
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await localStorage.GetItemAsync<string>("token");
            var identity = string.IsNullOrWhiteSpace(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(token.ParseClaimsFromJwt(), "jwt");

            httpClient.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
                ? null
                : new AuthenticationHeaderValue("bearer", token);

            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task Login(string token)
        {
            await localStorage.SetItemAsync("token", token);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public async Task Logout()
        {
            await localStorage.RemoveItemAsync("token");

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }

}
