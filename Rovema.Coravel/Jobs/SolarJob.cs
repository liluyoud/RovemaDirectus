using Coravel.Invocable;
using Dclt.Shared.Extensions;
using Rovema.Shared.Extensions;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Models;

namespace Rovema.Coravel.Jobs;

public class SolarJob(ILogger<SolarJob> logger, IRovemaService apiService) : IInvocable
{
    public async Task Invoke()
    {
        var rpas = await apiService.GetRpasSolarAsync();
        if (rpas != null)
        {
            var tasks = new List<Task>();
            foreach (var rpa in rpas)
            {
                tasks.Add(Read(rpa));
            }
            await Task.WhenAll(tasks);
            logger.LogInformation($"SolarJob executado às {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
        }
    }

    private async Task Read(RpaModel rpa)
    {
        var address = rpa.Settings.GetKey("address");
        if (!string.IsNullOrEmpty(address))
        {
            try {

                var solarData = await apiService.GetSolarAsync(address);
                if (solarData != null)
                {
                    var readSolar = solarData.ToCreateReadSolar(rpa);
                    await apiService.AddSolarAsync(readSolar);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Erro ao executar SolarJob {rpa.Name}: {ex.Message}");
            }

        }
    }

}
