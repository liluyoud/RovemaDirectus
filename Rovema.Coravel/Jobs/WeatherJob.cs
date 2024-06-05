using Coravel.Invocable;
using Rovema.Shared.Extensions;
using Rovema.Shared.Interfaces;

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
                var latitude = rpa.GetSetting("Latitude");
                var longitude = rpa.GetSetting("Longitude");
                if (latitude != null && longitude != null)
                {
                    tasks.Add(Read(rpa.Id, latitude, longitude));
                } else
                {
                   logger.LogError($"{rpa.Name} não processado: Lat {latitude} - Lon {longitude}");
                }
            }
            await Task.WhenAll(tasks);
            logger.LogInformation($"WeatherJob executado às {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
        }
    }

    private async Task Read(int rpaId, string latitude, string longitude)
    {
        var weather = await rovema.GetWeatherAsync(latitude, longitude);
        if (weather != null)
        {
            var read = weather.ToCreateReadWeather(rpaId);
            await rovema.AddWeatherAsync(read);
            // salvar cache
        }
    }

}
