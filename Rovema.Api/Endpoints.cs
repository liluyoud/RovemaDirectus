using Microsoft.Extensions.Caching.Distributed;
using Rovema.Shared.Extensions;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Api;

public static class Endpoints
{
    public static void MapRpaEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("rpas", async (
            RovemaService rovema,
            IDistributedCache cache,
            CancellationToken ct) =>
        {
            var rpas = await cache.GetAsync($"rpas",
                async token =>
                {
                    return await rovema.GetRpasAsync();
                },
                CacheOptions.DefaultExpiration,
                ct);

            return rpas is null ? Results.NotFound() : Results.Ok(rpas?.OrderBy(r => r.Name));
        }).Produces<List<RpaModel>>();

        app.MapGet("rpas/weather", async (
            RovemaService rovema,
            IDistributedCache cache,
            CancellationToken ct) =>
        {
            var weathers = await cache.GetAsync($"rpas",
               async token =>
               {
                   return await rovema.GetReadWeathersAsync();
               },
               CacheOptions.FiveMinutesExpiration,
               ct);
            return Results.Ok(weathers);
        }).Produces<List<ReadWeatherModel>>();
    }
}
