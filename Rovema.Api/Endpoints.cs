using Rovema.Shared.Models;
using Dclt.Directus;
using Rovema.Shared.Services;
using Rovema.Shared.Extensions;


namespace Rovema.Api;

public static class Endpoints
{
    public static void MapRpasEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("powerplant/{id}", async (long id, DirectusService directusService) => {
            var powerplant = await directusService.GetItemAsync<PowerPlantModel>("powerplants", id);

            if (powerplant != null)
            {
                var sumary = new PowerPlantSumaryModel();
                sumary.Id = powerplant.Id;
                sumary.Name = powerplant.Name;
                sumary.Type = powerplant.Type;
                sumary.Power = powerplant.Power ?? 0;

                int ionId = powerplant?.IonId ?? 0;
                var ion = await directusService.GetCache<ReadIonModel>(ionId);
                if (ion != null)
                {
                    sumary.IonId = powerplant?.IonId ?? 0;
                    sumary.IonKw = ion.Power_KW_Total ?? 0;
                    sumary.IonKVar = ion.Power_KVAR_Total ?? 0;
                    sumary.IonTensionV = ion.Voltage_Vll_Avg ?? 0;
                    sumary.IonKVar = ion.Power_KVAR_Total ?? 0;
                    sumary.IonFrequency = ion.Frequency ?? 0;

                    var diff = DateTime.Now - ion.ReadAt;
                    sumary.IonOk = (diff.TotalMinutes > 15) ? false : true;
                }

                int inverterId = powerplant?.InverterId ?? 0;
                var inverter = await directusService.GetCache<ReadInverterModel>(inverterId);
                if (inverter != null)
                {
                    sumary.InverterId = powerplant?.InverterId ?? 0;
                    sumary.InverterKw = inverter.Current ?? 0;
                    sumary.InverterDay = inverter.Day ?? 0;
                    sumary.InverterMonth = inverter.Month ?? 0;

                    var diff = DateTime.Now - inverter.ReadAt;
                    sumary.InverterOk = (diff.TotalMinutes > 15) ? false : true;
                }

                int solarId = powerplant?.SolarId ?? 0;
                var solar = await directusService.GetCache<ReadSolarModel>(solarId);
                if (solar != null)
                {
                    sumary.SolarId = powerplant?.SolarId ?? 0;
                    sumary.SolarKw = solar.Power1 ?? 0;

                    var diff = DateTime.Now - solar.ReadAt;
                    sumary.SolarOk = (diff.TotalMinutes > 15) ? false : true;

                }

                int weatherId = powerplant?.WeatherId ?? 0;
                var weather = await directusService.GetCache<ReadWeatherModel>(weatherId);
                if (weather != null)
                {
                    sumary.WeatherId = powerplant?.WeatherId ?? 0;
                    sumary.WeatherTempC = weather.TempC ?? 0;
                    sumary.WeatherDescrition = weather.Description;
                    sumary.WeatherClouds = weather.Clouds ?? 0;

                    var diff = DateTime.Now - weather.ReadAt;
                    sumary.WeatherOk = (diff.TotalMinutes > 30) ? false : true;
                }

                return Results.Ok(sumary);
            }
            return Results.NotFound();
        });

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
