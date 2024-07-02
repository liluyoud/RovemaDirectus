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
            .Filter("type", Operation.Equal, "Solar")
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
                var solarData = await readService.GetSolarAsync(address);
                if (solarData != null)
                {
                    CreateReadSolar readSolar = solarData.ToCreateReadSolar(rpa);
                    var panels = await directusService.GetPanels(rpa.Id);
                    var weather = await directusService.GetWeather(readSolar.WeatherId);
                    readSolar.BuildTeoricPower(panels, weather);
                    await directusService.CreateItemAsync("reads_solar", readSolar);

                    if (rpa.Id == 15) // pvh 1 -> create pvh 2
                    {
                        readSolar.RpaId = 24; // pvh 2
                        panels = await directusService.GetPanels(24); // pvh 2
                        readSolar.BuildTeoricPower(panels, weather);
                        await directusService.CreateItemAsync("reads_solar", readSolar);
                    }

                    if (rpa.Id == 14) // mac 1 -> mac 2 and mac 3
                    {
                        readSolar.RpaId = 22; // mac 2
                        panels = await directusService.GetPanels(22); // pvh 2
                        readSolar.BuildTeoricPower(panels, weather);
                        await directusService.CreateItemAsync("reads_solar", readSolar);

                        readSolar.RpaId = 23; // mac 3
                        panels = await directusService.GetPanels(23); // pvh 3
                        readSolar.BuildTeoricPower(panels, weather);
                        await directusService.CreateItemAsync("reads_solar", readSolar);
                    }
                }
                logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - SolarJob {rpa.Name} executado");
            }
            catch (Exception ex)
            {
                logger.LogError($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - Erro ao executar SolarJob {rpa.Name} {address}: {ex.Message}");
            }

        }
    }
}
