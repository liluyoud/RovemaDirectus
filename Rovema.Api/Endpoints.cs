using Rovema.Shared.Models;
using Dclt.Directus;
using Rovema.Shared.Services;
using Rovema.Shared.Extensions;


namespace Rovema.Api;

public static class Endpoints
{
    public static void MapRpasEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("reads/solar", async (CreateReadSolar read, DirectusService directusService, ReadService readService) => {
            var panels = await directusService.GetPanels(read.RpaId);
            var weather = await directusService.GetWeather(read.WeatherId);
            read.BuildTeoricPower(panels, weather);
            await directusService.CreateItemAsync("reads_solar", read);
        });
    }
}
