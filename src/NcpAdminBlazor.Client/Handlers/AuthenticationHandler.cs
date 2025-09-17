using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace NcpAdminBlazor.Client.Client.Handlers
{
    public class AuthenticationHandler(ILocalStorageService localStorage) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await localStorage.GetItemAsync<string>("token");
            
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}