using Coravel.Invocable;
using Dclt.Shared.Extensions;
using Rovema.Shared.Extensions;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Models;

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
                    logger.LogError($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - {rpa.Name} não processado: Lat {latitude} , Lon {longitude}");
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
            logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - WeatherJob {rpa.Name} executado");
        }
        catch (Exception ex)
        {
            logger.LogError($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - Erro ao executar WeatherJob {rpa.Name}: {ex.Message}");

        }
    }

}
