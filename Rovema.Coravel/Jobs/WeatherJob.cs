using Coravel.Invocable;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Models;

namespace Rovema.Coravel.Jobs;

public class WeatherJob (ILogger<WeatherJob> logger, IRovemaService rovema) : IInvocable
{
    public async Task Invoke()
    {
        var rpas = await rovema.GetRpasAsync("Tempo");
        if (rpas != null)
        {
            var tasks = new List<Task<ReadWeatherModel>>();
            foreach (var rpa in rpas)
            {
                var latitude = rpa.GetSetting("Latitude");
                var longitude = rpa.GetSetting("Longitude");
                if (latitude != null && longitude != null)
                {
                    tasks.Add(rovema.GetWeatherAsync(rpa.Id, latitude, longitude));
                } else
                {
                   logger.LogError($"{rpa.Name} não processado: Lat {latitude} - Lon {longitude}");
                }
            }
            await Task.WhenAll(tasks);
            foreach (var task in tasks)
            {
                logger.LogInformation($"{task.Result.TempC}");
            }
            logger.LogInformation($"WeatherJob executado às {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
        }
    }

}
