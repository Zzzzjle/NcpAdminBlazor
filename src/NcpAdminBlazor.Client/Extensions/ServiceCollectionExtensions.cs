using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using MudBlazor.Translations;
using NcpAdminBlazor.Client.Kiota;
using NcpAdminBlazor.Client.Stores;

namespace NcpAdminBlazor.Client.Extensions;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddClientServices()
        {
            services.AddScoped<MenuProvider>();
            services.AddScoped<LayoutStore>();
            services.AddScoped<BreadcrumbService>();
            return services;
        }

        public IServiceCollection AddKiotaClient()
        {
            services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
            services.AddScoped<IAuthenticationProvider>(sp =>
            {
                var accessTokenProvider = sp.GetRequiredService<IAccessTokenProvider>();
                var baseAuthProvider = new BaseBearerTokenAuthenticationProvider(accessTokenProvider);
                return new BearerTokenAuthenticationProvider(baseAuthProvider);
            });
            services.AddScoped<IRequestAdapter, HttpClientRequestAdapter>();
            services.AddScoped<ApiClient>();
            services.AddScoped<ApiWrapper>();
            return services;
        }

        public void AddAuthenticationAndLocalization()
        {
            services.AddOptions();
            services.AddAuthorizationCore();
            services.AddBlazoredLocalStorage();
            services.AddCascadingAuthenticationState();
            services.AddScoped<IAuthTokenService, LocalStorageAuthTokenService>();
            services.AddScoped<TokenAuthenticationStateProvider>();
            services.AddScoped<AuthenticationStateProvider>(sp =>
                sp.GetRequiredService<TokenAuthenticationStateProvider>());
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddMudTranslations();
        }
    }
}