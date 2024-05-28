using Microsoft.Extensions.Caching.Distributed;
using Dclt.Shared.Services;
using Rovema.Shared.Models;

namespace Rovema.Api;

public static class Endpoints
{
    public static void MapRpasEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("rpas", async (DirectusService directus) => {
            //var directus = new DirectusClient(baseUrl, accessToken);
            var rpa = await directus.GetItemsAsync<IEnumerable<RpaModel>>("rpas");
            return rpa is null ? Results.NotFound() : Results.Ok(rpa);

        }).Produces<IEnumerable<RpaModel>>();

        app.MapGet("rpas/{id}", async (long id, DirectusService directus) => {
            //var directus = new DirectusClient(baseUrl, accessToken);
            var rpa = await directus.GetItemAsync<RpaModel>("rpas", id);
            return rpa is null ? Results.NotFound() : Results.Ok(rpa);

        }).Produces<RpaModel>();

        app.MapPost("rpas", async (RpaModel item, DirectusService directus) => {
            //var directus = new DirectusClient(baseUrl, accessToken);
            var rpa = await directus.CreateItemAsync("rpas", item);
            return rpa is null ? Results.NoContent() : Results.Ok(rpa);

        }).Produces<RpaModel>();

        app.MapPatch("rpas", async (long id, RpaModel item, DirectusService directus) => {
            //var directus = new DirectusClient(baseUrl, accessToken);
            var rpa = await directus.UpdateItemAsync("rpas", id, item);
            return rpa is null ? Results.NoContent() : Results.Ok(rpa);

        }).Produces<RpaModel>();

        app.MapDelete("rpas", async (long id, DirectusService directus) => {
            //var directus = new DirectusClient(baseUrl, accessToken);
            return await directus.DeleteItemAsync("rpas", id);
        }).Produces<bool>();


        app.MapGet("cached/{collection}/{id}", async (string collection, long id, DirectusService directus, IDistributedCache cache) => {
            //var directus = new DirectusClient(baseUrl, accessToken);
            var rpa = await directus.GetCachedItemAsync<RpaModel>(collection, id, cache, 5);
            return rpa is null ? Results.NotFound() : Results.Ok(rpa);

        }).Produces<RpaModel>();
    }


}
