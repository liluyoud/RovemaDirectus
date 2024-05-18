using Dclt.Shared.Extensions;
using Dclt.Shared.Services;
using Microsoft.Extensions.Caching.Distributed;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Api;

public static class Endpoints
{

    public static void MapRpaEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("weather", async (int id, string latitude, string longitude,
            HttpServices services, IDistributedCache cache, CancellationToken ct) => {

            var weather = await cache.GetAsync($"weather-{id}", async token => {
                var weather = await services.GetWeatherAsync(latitude, longitude);
                // falta preencher o rpaid
                //var readWeather = openWeather.ToWeather();
                //await cache.SetObjectAsync("rpa-" + weather?.RpaId, weather);
                return weather;
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
    }
}
