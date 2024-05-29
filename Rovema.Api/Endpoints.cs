using Dclt.Shared.Models;
using Dclt.Shared.Services;
using Rovema.Shared.Contracts;
using Rovema.Shared.Models;

namespace Rovema.Api;

public static class Endpoints
{
    public static void MapRpasEndpoints(this IEndpointRouteBuilder app)
    {
        #region Rovema
        app.MapGet("rpas/weather", async (DirectusService directus) => {
            var rpas = await directus.GetCachedItemsAsync<IEnumerable<RpaModel>>("rpas", 5, "[type][_eq]=Tempo");
            return rpas is null ? Results.NotFound() : Results.Ok(rpas);

        }).Produces<IEnumerable<RpaModel>>();

        app.MapGet("weather", async (string latitude, string longitude, HttpService http) => {
            var weather = await http.GetCachedWeatherAsync(latitude, longitude);
            return weather is null ? Results.NotFound() : Results.Ok(weather);

        }).Produces<WeatherModel>();

        app.MapPost("reads/weather", async (CreateReadWeather read, DirectusService directus) => {
            var item = await directus.CreateItemAsync<CreateReadWeather, ReadWeatherModel>("reads_weather", read);
            return item is null ? Results.NoContent() : Results.Ok(item);

        }).Produces<ReadWeatherModel>();

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
        #endregion

        app.MapDelete("rpas", async (long id, DirectusService directus) => {
            return await directus.DeleteItemAsync("rpas", id);
        }).Produces<bool>();


        #region Cached

        app.MapGet("cached/{collection}/{id}", async (string collection, long id, DirectusService directus) => {
            var rpa = await directus.GetCachedItemAsync<RpaModel>(collection, id, 5);
            return rpa is null ? Results.NotFound() : Results.Ok(rpa);

        }).Produces<RpaModel>();

        app.MapGet("cached/{collection}", async (string collection, DirectusService directus) => {
            var rpas = await directus.GetCachedItemsAsync<IEnumerable<RpaModel>>(collection, 5);
            return rpas is null ? Results.NotFound() : Results.Ok(rpas);

        }).Produces<RpaModel>();

        #endregion
    }


}
