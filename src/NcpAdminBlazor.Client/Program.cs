using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using NcpAdminBlazor.Client.Client.Apis;
using NcpAdminBlazor.Client.Client.Providers;
using NcpAdminBlazor.Client.Client.Services;
using Refit;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

// Services注册
builder.Services.AddRefitClient<IUserApi>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<TokenAuthenticationStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddSingleton<AuthenticationStateProvider, TokenAuthenticationStateProvider>();
builder.Services.AddScoped<IUserService, UserService>();

await builder.Build().RunAsync();
