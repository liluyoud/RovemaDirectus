using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace Rovema.Shared.Extensions;

public static class RefitExtensions
{
    public static void AddRefit<T>(this IServiceCollection services, string url) where T : class
    {
        services.AddRefitClient<T>().ConfigureHttpClient(h =>
        {
            h.BaseAddress = new Uri(url);
        });
    }
}
