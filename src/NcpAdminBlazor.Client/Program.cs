using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using MudBlazor.Services;
using NcpAdminBlazor.Client;
using NcpAdminBlazor.Client.Providers;
using NcpAdminBlazor.Client.States;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

// 配置 HttpClient
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// 配置 Kiota ApiClient
builder.Services.AddScoped<IRequestAdapter>(sp =>
{
    var httpClient = sp.GetRequiredService<HttpClient>();
    var authProvider = new AnonymousAuthenticationProvider(); // 或其他认证提供器
    return new HttpClientRequestAdapter(authProvider, httpClient: httpClient);
});

builder.Services.AddScoped<ApiClient>();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<TokenAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, TokenAuthenticationStateProvider>();

builder.Services.AddScoped<LayoutState>();
builder.Services.AddScoped<BreadcrumbState>();

await builder.Build().RunAsync();