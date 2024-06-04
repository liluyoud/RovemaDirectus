using Dclt.Shared.Models;
using Dclt.Shared.Services;
using Dclt.Shared.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Rovema.Shared.Contracts;
using Rovema.Shared.Models;

namespace Rovema.Api;

public static class Endpoints
{
    public static void MapRpasEndpoints(this IEndpointRouteBuilder app)
    {
        #region Rovema
        app.MapGet("rpas/weather", async (DirectusService directus) => {
            var rpas = await directus.GetCachedItemsAsync<IEnumerable<RpaModel>>("weather", "rpas", 5, "{ \"_and\": [ { \"type\": { \"_eq\": \"Tempo\" } }, { \"status\": { \"_eq\": \"published\" } } ] }");
            return rpas is null ? Results.NotFound() : Results.Ok(rpas);

        }).Produces<IEnumerable<RpaModel>>();

        app.MapGet("rpas/ion", async (DirectusService directus) => {
            var rpas = await directus.GetCachedItemsAsync<IEnumerable<RpaModel>>("ion", "rpas", 5, "{ \"_and\": [ { \"type\": { \"_eq\": \"Ion\" } }, { \"status\": { \"_eq\": \"published\" } } ] }");
            return rpas is null ? Results.NotFound() : Results.Ok(rpas);

        }).Produces<IEnumerable<RpaModel>>();

        app.MapGet("weather", async (string latitude, string longitude, HttpService http) => {
            var weather = await http.GetCachedWeatherAsync(latitude, longitude);
            return weather is null ? Results.NotFound() : Results.Ok(weather);

        }).Produces<WeatherModel>();

        app.MapPost("reads/weather", async (CreateReadWeather read, DirectusService directus, IDistributedCache cache) => {
            var item = await directus.CreateItemAsync<CreateReadWeather, ReadWeatherModel>("reads_weather", read);
            await cache.SetObjectAsync($"rpa-weather-{read.RpaId}", item);
            return item is null ? Results.NoContent() : Results.Ok(item);

        }).Produces<ReadWeatherModel>();

        app.MapPost("reads/ion", async (CreateReadIon read, DirectusService directus, IDistributedCache cache) => {
            var item = await directus.CreateItemAsync<CreateReadIon, ReadIonModel>("reads_ion", read);
            await cache.SetObjectAsync($"rpa-ion-{read.RpaId}", item);
            return item is null ? Results.NoContent() : Results.Ok(item);

        }).Produces<ReadIonModel>();

        #endregion

        #region CRUD
        app.MapGet("rpas", async (DirectusService directus) => {
            var rpa = await directus.GetItemsAsync<IEnumerable<RpaModel>>("rpas");
            return rpa is null ? Results.NotFound() : Results.Ok(rpa);

        }).Produces<IEnumerable<RpaModel>>();

        app.MapGet("rpas/{id}", async (long id, DirectusService directus) => {
            var rpa = await directus.GetItemAsync<RpaModel>("rpas", id);
            return rpa is null ? Results.NotFound() : Results.Ok(rpa);

        }).Produces<RpaModel>();

        app.MapPost("rpas", async (CreateRpa item, DirectusService directus) => {
            var rpa = await directus.CreateItemAsync<CreateRpa, RpaModel>("rpas", item);
            return rpa is null ? Results.NoContent() : Results.Ok(rpa);

        }).Produces<RpaModel>();

        app.MapPatch("rpas", async (long id, RpaModel item, DirectusService directus) => {
            var rpa = await directus.UpdateItemAsync("rpas", id, item);
            return rpa is null ? Results.NoContent() : Results.Ok(rpa);

        }).Produces<RpaModel>();

        app.MapDelete("rpas", async (long id, DirectusService directus) => {
            return await directus.DeleteItemAsync("rpas", id);
        }).Produces<bool>();
        #endregion
    }

}
