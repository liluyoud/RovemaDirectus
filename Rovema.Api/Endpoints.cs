using Rovema.Shared.Models;
using Dclt.Directus;
using Rovema.Shared.Services;
using Rovema.Shared.Extensions;


namespace Rovema.Api;

public static class Endpoints
{
    public static void MapRpasEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("cache/{id}", async (long id, DirectusService directusService) => {
            var cache = await directusService.GetCache<dynamic>(id);
            return Results.Ok(cache);
        });

        app.MapPost("reads/solar", async (CreateReadSolar read, DirectusService directusService, ReadService readService) => {
            var panels = await directusService.GetPanels(read.RpaId);
            var weather = await directusService.GetCache<ReadWeatherModel>(read.RpaId);
            read.BuildTeoricPower(panels, weather);
            await directusService.CreateItemAsync("reads_solar", read);
        });
    }
}
