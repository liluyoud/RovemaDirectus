using Dclt.Shared.Models;
using Dclt.Shared.Services;
using Dclt.Shared.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Rovema.Shared.Contracts;
using Rovema.Shared.Models;
using Rovema.Shared.Services;
using Rovema.Shared.Extensions;

namespace Rovema.Api;

public static class Endpoints
{
    public static void MapRpasEndpoints(this IEndpointRouteBuilder app)
    {
        #region Read Cache
        app.MapGet("cache/weather/{rpaId}", async (int rpaId, IDistributedCache cache) => {
            var weather = await cache.GetObjectAsync<ReadWeatherModel>($"rpa-weather-{rpaId}");
            return weather is null ? Results.NotFound() : Results.Ok(weather);
        }).Produces<ReadWeatherModel>();
        #endregion

        #region Get Rpas
        app.MapGet("rpas/weather", async (DirectusService directus) => {
            var rpas = await directus.GetCachedItemsAsync<IEnumerable<RpaModel>>("weather", "rpas", 5, "{ \"_and\": [ { \"type\": { \"_eq\": \"Tempo\" } }, { \"status\": { \"_eq\": \"published\" } } ] }");
            return rpas is null ? Results.NotFound() : Results.Ok(rpas);

        }).Produces<IEnumerable<RpaModel>>();

        app.MapGet("rpas/ion", async (DirectusService directus) => {
            var filter = "{ \"_and\": [ { \"type\": { \"_eq\": \"Ion\" } }, { \"status\": { \"_eq\": \"published\" } } ] }";
            var rpas = await directus.GetCachedItemsAsync<IEnumerable<RpaModel>>("ion", "rpas", 5, filter);
            return rpas is null ? Results.NotFound() : Results.Ok(rpas);

        }).Produces<IEnumerable<RpaModel>>();

        app.MapGet("rpas/solar", async (DirectusService directus) => {
            var filter = "{ \"_and\": [ { \"type\": { \"_eq\": \"Solarimetrica\" } }, { \"status\": { \"_eq\": \"published\" } } ] }";
            var rpas = await directus.GetCachedItemsAsync<IEnumerable<RpaModel>>("solar", "rpas", 5, filter);
            return rpas is null ? Results.NotFound() : Results.Ok(rpas);

        }).Produces<IEnumerable<RpaModel>>();

        app.MapGet("rpas/inverter", async (DirectusService directus) => {
            var filter = "{ \"_and\": [ { \"type\": { \"_eq\": \"Inversor\" } }, { \"status\": { \"_eq\": \"published\" } } ] }";
            var rpas = await directus.GetCachedItemsAsync<IEnumerable<RpaModel>>("inverter", "rpas", 5, filter);
            return rpas is null ? Results.NotFound() : Results.Ok(rpas);

        }).Produces<IEnumerable<RpaModel>>();
        #endregion

        #region Read Controlers

        app.MapGet("ion", async (string address, bool reverse, ReadService read) => {
            var ion = await read.GetCachedIonAsync(address, reverse);
            return ion is null ? Results.NotFound() : Results.Ok(ion);

        }).Produces<IonModel>();

        app.MapGet("weather", async (string latitude, string longitude, HttpService http) => {
            var weather = await http.GetCachedWeatherAsync(latitude, longitude);
            return weather is null ? Results.NotFound() : Results.Ok(weather);

        }).Produces<WeatherModel>();

        app.MapGet("solar", async (string address, ReadService read) => {
            var solar = await read.GetCachedSolarAsync(address);
            return solar is null ? Results.NotFound() : Results.Ok(solar);

        }).Produces<List<KeyValueModel>>();

        #endregion

        #region Post Reads
        
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

        app.MapPost("reads/inverter", async (CreateReadInverter read, DirectusService directus, IDistributedCache cache) => {
            var item = await directus.CreateItemAsync<CreateReadInverter, ReadInverterModel>("reads_inverter", read);
            await cache.SetObjectAsync($"rpa-inverter-{read.RpaId}", item);
            return item is null ? Results.NoContent() : Results.Ok(item);

        }).Produces<ReadInverterModel>();

        app.MapPost("reads/solar", async (CreateReadSolar read, DirectusService directus, ReadService readService, IDistributedCache cache) => {

            var panels = await readService.GetCachedSolarPanelsAsync(read.RpaId);
            var weather = await cache.GetObjectAsync<ReadWeatherModel>($"rpa-weather-{read.WeatherId}");
            read.BuildTeoricPower(panels, weather);
            var item = await directus.CreateItemAsync<CreateReadSolar, ReadSolarModel>("reads_solar", read);
            await cache.SetObjectAsync($"rpa-solar-{read.RpaId}", item);
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
