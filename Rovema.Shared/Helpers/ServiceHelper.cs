using Dclt.Directus;
using Dclt.Services.OpenWeather;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Dclt.Services.Helpers;
using Rovema.Shared.Services;

namespace Rovema.Shared.Helpers;

public static class ServiceHelper
{
    public static void AddRovemaServices(this IServiceCollection services, IConfiguration conf)
    {
        services.AddRedis(conf);
        services.AddHttpClient();
        services.AddScoped<DirectusService>();
        services.AddScoped<OpenWeatherService>();
        services.AddScoped<ReadService>();
    }
}
