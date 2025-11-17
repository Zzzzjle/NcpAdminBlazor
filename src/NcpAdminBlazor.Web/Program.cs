using MudBlazor.Services;
using NcpAdminBlazor.Client.Stores;
using NcpAdminBlazor.Web.Components;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Forwarder;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddScoped<LayoutStore>();

builder.Services.AddOutputCache();

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new("https+http://apiservice") });

builder.Services.AddHttpForwarder();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Forward API requests to the API service
app.MapForwarder("/api/{**catch-all}", "http+https://apiservice/");

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(NcpAdminBlazor.Client._Imports).Assembly)
    .AllowAnonymous();

app.MapDefaultEndpoints();

await app.RunAsync();