using Microsoft.Extensions.Caching.Distributed;
using Rovema.Shared.Models;
using Dclt.Services.Cache;


namespace Rovema.Api;

public static class Endpoints
{
    public static void MapRpasEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("cache/weather/{rpaId}", async (int rpaId, IDistributedCache cache) => {
            var weather = await cache.GetObjectAsync<ReadWeatherModel>($"rpa-weather-{rpaId}");
            return weather is null ? Results.NotFound() : Results.Ok(weather);
        }).Produces<ReadWeatherModel>();


    }
}
