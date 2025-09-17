using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using NcpAdminBlazor.Client.Client.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();
builder.Services.AddHttpClient("MudBlazorWithJwt.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MudBlazorWithJwt.ServerAPI"));


builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<TokenAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<TokenAuthenticationStateProvider>());


await builder.Build().RunAsync();
