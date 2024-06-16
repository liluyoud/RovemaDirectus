using Coravel.Invocable;
using Dclt.Shared.Extensions;
using Rovema.Shared.Extensions;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Models;
using Rovema.Shared.Services;

namespace Rovema.Coravel.Jobs;

public class SolarJob(ILogger<SolarJob> logger, IRovemaService apiService, ReadService readService) : IInvocable
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
                    var readSolar = solarData.ToCreateReadSolar(rpa);
                    await apiService.AddSolarAsync(readSolar);
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
