using Microsoft.Extensions.Caching.Distributed;
using Rovema.Shared.Extensions;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Api;

public static class Endpoints
{

    public static void MapRpaEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("weather", async (int id, string latitude, string longitude, 
            RovemaService rovema, IDistributedCache cache, CancellationToken ct) => {

            var weather = await cache.GetAsync($"weather-{id}", async token => {
                var openWeather = await rovema.GetWeatherAsync(latitude, longitude);
                return openWeather.GetRead(id);
            }, CacheOptions.FiveMinutesExpiration, ct);

            return weather is null ? Results.NotFound() : Results.Ok(weather);

        }).Produces<ReadWeatherModel>();

        app.MapGet("rpas", async (RovemaService rovema, IDistributedCache cache, CancellationToken ct) => {

            var rpas = await cache.GetAsync("rpas", async token => {
                return await rovema.GetRpasAsync();
            }, CacheOptions.FiveMinutesExpiration, ct);

            return rpas is null ? Results.NotFound() : Results.Ok(rpas);

        }).Produces<List<RpaModel>>();

        app.MapGet("rpas/{type}", async (string type, 
            RovemaService rovema, IDistributedCache cache, CancellationToken ct) => {
            
                var rpas = await cache.GetAsync($"rpas{type}", async token => {
                return await rovema.GetRpasAsync(type);
            }, CacheOptions.FiveMinutesExpiration, ct);

            return rpas is null ? Results.NotFound() : Results.Ok(rpas);

        }).Produces<List<RpaModel>>();

        //app.MapGet("rpas/readweather", async (
        //    RovemaService rovema,
        //    IDistributedCache cache,
        //    CancellationToken ct) =>
        //{
        //    var weathers = await cache.GetAsync($"weathers",
        //       async token =>
        //       {
        //           return await rovema.GetReadWeathersAsync();
        //       },
        //       CacheOptions.FiveMinutesExpiration,
        //       ct);
        //    return Results.Ok(weathers);
        //}).Produces<List<ReadWeatherModel>>();
    }
}
