using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using NcpAdminBlazor.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddMudServices();

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddKiotaClient();

builder.Services.AddAuthenticationAndLocalization();
builder.Services.AddClientServices();

var host = builder.Build();

await host.SetCulture();

await host.RunAsync();