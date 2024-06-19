using Coravel.Invocable;
using Dclt.Directus;
using Dclt.Shared.Extensions;
using Rovema.Shared.Extensions;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Coravel.Jobs;

public class SolarJob(ILogger<SolarJob> logger, DirectusService directusService, ReadService readService) : IInvocable
{
    public async Task Invoke()
    {
        var query = new DirectusQuery()
            .Fields("id,name,type,settings")
            .Filter("type", Operation.Equal, "Solarimetrica")
            .Filter("status", Operation.Equal, "published")
            .Build();

        var rpas = await directusService.GetItemsAsync<IEnumerable<RpaModel>>("rpas", query);
        if (rpas != null)
        {
            var tasks = new List<Task>();
            foreach (var rpa in rpas)
            {
                tasks.Add(Read(rpa));
            }
            await Task.WhenAll(tasks);
        }
    }

    private async Task Read(RpaModel rpa)
    {
        var address = rpa.Settings.GetKey("address");
        if (!string.IsNullOrEmpty(address))
        {
            try {

                //var solarData = await apiService.GetSolarAsync(address);
                var solarData = await readService.GetSolarAsync(address);
                if (solarData != null)
                {
                    CreateReadSolar readSolar = solarData.ToCreateReadSolar(rpa);
                    var panels = await GetPanels(rpa.Id);
                    var weather = await GetWeather(readSolar.WeatherId);
                    readSolar.BuildTeoricPower(panels, weather);
                    await directusService.CreateItemAsync("reads_solar", readSolar);
                }
                logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - SolarJob {rpa.Name} executado");
            }
            catch (Exception ex)
            {
                logger.LogError($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - Erro ao executar SolarJob {rpa.Name} {address}: {ex.Message}");
            }

        }
    }

    private async Task<IEnumerable<SolarPanelModel>?> GetPanels(int rpaId)
    {
        var query = new DirectusQuery()
            .Filter("rpa_id", Operation.Equal, rpaId)
            .Filter("status", Operation.Equal, "published")
            .Build();
        return await directusService.GetItemsAsync<IEnumerable<SolarPanelModel>>("solar_panels", query);
    }

    private async Task<ReadWeatherModel?> GetWeather(int? rpaId)
    {
        if (rpaId == null) return null;
        var query = new DirectusQuery()
            .Filter("rpa_id", Operation.Equal, rpaId)
            .Sort("-date_created")
            .Limit(1)
            .Build();
        var weather = await directusService.GetItemsAsync<IEnumerable<ReadWeatherModel>>("reads_weather", query);
        return weather?.First();
    }
}
