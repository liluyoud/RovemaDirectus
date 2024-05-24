using Coravel.Invocable;
using Dclt.Shared.Services;
using Rovema.Shared.Extensions;
using Rovema.Shared.Interfaces;

namespace Rovema.Coravel.Jobs;

public class WeatherJob (ILogger<WeatherJob> logger, IRovemaService rovema, HttpServices services) : IInvocable
{
    public async Task Invoke()
    {
        var rpas = await rovema.GetRpasAsync("Tempo");
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
            logger.LogInformation($"WeatherJob executado às {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
        }
    }

    private async Task Read(int rpaId, string latitude, string longitude)
    {
        var weather = await services.GetWeatherAsync(latitude, longitude);
        if (weather != null)
        {
            var read = weather.ToReadModel(rpaId);
            // salvar na lista
            //salvar cache
        }
    }

}
