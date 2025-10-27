using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using MudBlazor.Services;
using MudBlazor.Translations;
using NcpAdminBlazor.Client;
using NcpAdminBlazor.Client.Extensions;
using NcpAdminBlazor.Client.HttpClient;
using NcpAdminBlazor.Client.HttpClient.Auth;
using NcpAdminBlazor.Client.Providers;
using NcpAdminBlazor.Client.Services;
using NcpAdminBlazor.Client.Stores;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

#region 配置 Kiota ApiClient

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IAccessTokenProvider, AccessTokenProvider>();
builder.Services.AddScoped<IAuthenticationProvider>(sp =>
{
    var accessTokenProvider = sp.GetRequiredService<IAccessTokenProvider>();
    var baseAuthProvider = new BaseBearerTokenAuthenticationProvider(accessTokenProvider);
    return new BearerTokenAuthenticationProvider(baseAuthProvider);
});
builder.Services.AddScoped<IRequestAdapter, HttpClientRequestAdapter>();
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<ApiWrapper>();

#endregion

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IAuthTokenService, LocalStorageAuthTokenService>();
builder.Services.AddScoped<TokenAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<TokenAuthenticationStateProvider>());
builder.Services.AddScoped<MenuProvider>();
builder.Services.AddScoped<LayoutStore>();
builder.Services.AddScoped<BreadcrumbStore>();

builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddMudTranslations();

var host = builder.Build();

await host.SetCulture();

await host.RunAsync();