using Dclt.Shared.Helpers;
using Dclt.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rovema.Shared.Services;

namespace Rovema.Shared.Helpers;

public static class ServiceHelper
{
    public static void AddRovemaServices(this IServiceCollection services, IConfiguration conf)
    {
        services.AddRedis(conf);
        services.AddHttpClient();
        services.AddTransient<HttpService>();
        services.AddTransient<DirectusService>();
        services.AddTransient<ReadService>();
    }
}
