using Coravel.Invocable;
using Dclt.Shared.Extensions;
using Rovema.Shared.Extensions;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Models;
using System.Net;

namespace Rovema.Coravel.Jobs;

public class WeatherJob (ILogger<WeatherJob> logger, IRovemaService rovema) : IInvocable
{
    public async Task Invoke()
    {
        var rpas = await rovema.GetRpasWeatherAsync();
        if (rpas != null)
        {
            var tasks = new List<Task>();
            foreach (var rpa in rpas)
            {
                var latitude = rpa.Settings.GetKey("Latitude");
                var longitude = rpa.Settings.GetKey("Longitude");
                if (latitude != null && longitude != null)
                {
                    tasks.Add(Read(rpa, latitude, longitude));
                } else
                {
                   logger.LogError($"{rpa.Name} não processado: Lat {latitude} - Lon {longitude}");
                }
            }
            await Task.WhenAll(tasks);
        }
    }

    private async Task Read(RpaModel rpa, string latitude, string longitude)
    {
        try
        {
            var weather = await rovema.GetWeatherAsync(latitude, longitude);
            if (weather != null)
            {
                var read = weather.ToCreateReadWeather(rpa.Id);
                await rovema.AddWeatherAsync(read);
            }
            logger.LogInformation($"WeatherJob {rpa.Name} executado às {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");

        }
        catch (Exception ex)
        {
            logger.LogError($"Erro ao executar IonJob {rpa.Name} {latitude}-{longitude}: {ex.Message}");

        }
    }

}
