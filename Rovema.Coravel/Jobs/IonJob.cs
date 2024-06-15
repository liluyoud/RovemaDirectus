﻿using Coravel.Invocable;
using Rovema.Shared.Interfaces;
using Dclt.Shared.Extensions;
using Rovema.Shared.Extensions;
using Rovema.Shared.Models;

namespace Rovema.Coravel.Jobs;

public class IonJob(ILogger<IonJob> logger, IRovemaService rovema) : IInvocable
{
    public async Task Invoke()
    {
        var rpas = await rovema.GetRpasIonAsync();
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
                var ionRead = await rovema.GetIonAsync(address, reverse);
                if (ionRead != null)
                {
                    var read = ionRead.ToCreateReadIon(rpa.Id, type);
                    await rovema.AddIonAsync(read);
                }
                logger.LogInformation($"IonJob {rpa.Name} executado às {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

            }
            catch (Exception ex) 
            {
                logger.LogError($"Erro ao executar IonJob {rpa.Name} {address}: {ex.Message}");
            }
        }
    }

}
