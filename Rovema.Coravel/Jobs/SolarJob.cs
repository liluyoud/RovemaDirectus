using Coravel.Invocable;
using Rovema.Shared.Extensions;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Coravel.Jobs;

public class SolarJob(ILogger<SolarJob> logger, IRovemaService rovema, ReadService readService) : IInvocable
{
    public async Task Invoke()
    {
        var rpas = await rovema.GetRpasSolarAsync();
        if (rpas != null)
        {
            var tasks = new List<Task>();
            foreach (var rpa in rpas)
            {
                var address = rpa.GetSetting("address");
                var weatherId = rpa.GetSetting("weatherId");
                var pir1 = rpa.GetSetting("pir1");
                var pir2 = rpa.GetSetting("pir2");
                var pir3 = rpa.GetSetting("pir3");
                var tempAir = rpa.GetSetting("tempAir");
                var humidity = rpa.GetSetting("humidity");
                var windSpeed = rpa.GetSetting("windSpeed");
                if (address != null && weatherId != null && pir1 != null)
                {
                    tasks.Add(Read(rpa));
                }
            }
            await Task.WhenAll(tasks);
            logger.LogInformation($"SolarJob executado às {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
        }
    }

    private async Task Read(RpaModel rpa)
    {
        var address = rpa.GetSetting("address");
        if (!string.IsNullOrEmpty(address))
        {
            try {

                var solarData = await readService.GetCachedSolarAsync(address);
                if (solarData != null)
                {
                    var readSolar = solarData.ToCreateReadSolar(rpa);

                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Erro ao executar SolarJob {rpa.Name}: {ex.Message}");
            }

        }
    }

}
