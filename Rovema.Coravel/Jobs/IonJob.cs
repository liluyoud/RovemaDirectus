using Coravel.Invocable;
using Dclt.Shared.Extensions;
using Rovema.Shared.Extensions;
using Rovema.Shared.Models;
using Rovema.Shared.Services;
using Dclt.Directus;
using System;

namespace Rovema.Coravel.Jobs;

public class IonJob(ILogger<IonJob> logger, DirectusService directusService, ReadService readService) : IInvocable
{
    public async Task Invoke()
    {
        var query = new DirectusQuery()
            .Fields("id,name,type,settings")
            .Filter("type", Operation.Equal, "Ion")
            .Filter("status", Operation.Equal, "published")
            .Build();

        var rpas = await directusService.GetItemsAsync<IEnumerable<RpaModel>>("rpas", query);
        if (rpas != null)
        {
            var tasks = new List<Task>();
            foreach (var rpa in rpas)
            {
                var primary = rpa.Settings.GetKey("primary");
                var secondary = rpa.Settings.GetKey("secondary");
                var reverseStr = rpa.Settings.GetKey("reverse");
                var reverse = reverseStr == null || reverseStr == "false" ? false : true;
                if (primary != null)
                {
                    tasks.Add(Read(rpa, primary, "Primário", reverse));
                }
                if (secondary != null)
                {
                    tasks.Add(Read(rpa, secondary, "Secundário", reverse));
                }
            }
            await Task.WhenAll(tasks);
        }
    }

    private async Task Read(RpaModel rpa, string address, string type, bool reverse)
    {
        if (!string.IsNullOrEmpty(address))
        {
            try
            {
                var ionRead = await readService.GetIonAsync(address, reverse);

                if (ionRead != null)
                {
                    var readIon = ionRead.ToCreateReadIon(rpa.Id, type);
                    await directusService.CreateItemAsync("reads_ion", readIon);
                    logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - IonJob {rpa.Name} executado");
                }
            }
            catch (Exception ex) 
            {
                logger.LogError($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - Erro ao executar IonJob {rpa.Name} {address}: {ex.Message}");

            }
        }
    }

}
