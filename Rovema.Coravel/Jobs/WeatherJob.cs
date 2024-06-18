using Coravel.Invocable;
using Dclt.Directus;
using Dclt.Services.OpenWeather;
using Dclt.Shared.Extensions;
using Rovema.Shared.Extensions;
using Rovema.Shared.Models;

namespace Rovema.Coravel.Jobs;

public class WeatherJob (ILogger<WeatherJob> logger, DirectusService directusService, OpenWeatherService weatherService) : IInvocable
{
    public async Task Invoke()
    {
        var query = new Query()
            .Fields("id,name,type,settings")
            .Filter("type", Operation.Equal, "Tempo")
            .Filter("status", Operation.Equal, "published")
            .Build();

        var rpas = await directusService.GetItemsAsync<IEnumerable<RpaModel>>("rpas", query);
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
            var weather = await weatherService.GetWeatherAsync(latitude, longitude);
            if (weather != null)
            {
                var readWeather = weather.ToCreateReadWeather(rpa.Id);
                await directusService.CreateItemAsync("reads_weather", readWeather);
            }
            logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - WeatherJob {rpa.Name} executado");
        }
        catch (Exception ex)
        {
            logger.LogError($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} - Erro ao executar WeatherJob {rpa.Name}: {ex.Message}");

        }
    }

}
