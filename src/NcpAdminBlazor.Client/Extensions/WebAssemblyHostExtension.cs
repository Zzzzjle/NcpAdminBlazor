using System.Globalization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace NcpAdminBlazor.Client.Extensions;

public static class WebAssemblyHostExtension
{
    /// <summary>
    /// set culture from local storage or default culture
    /// </summary>
    /// <param name="host"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task SetCulture(this WebAssemblyHost host)
    {
        var configuration = host.Services.GetRequiredService<IConfiguration>();
        var localStorageKey = configuration["Culture:LocalStorageKey"] ??
                              throw new InvalidOperationException("Culture:LocalStorageKey configuration is missing.");

        var localStorageService = host.Services.GetRequiredService<ILocalStorageService>();
        var result = await localStorageService.GetItemAsync<string>(localStorageKey);

        CultureInfo culture;
        if (!string.IsNullOrWhiteSpace(result))
        {
            culture = new CultureInfo(result);
        }
        else
        {
            var defaultCulture = configuration["Culture:DefaultCulture"] ?? throw new InvalidOperationException(
                "Culture:DefaultCulture configuration is missing.");
            culture = new CultureInfo(defaultCulture);
        }

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}