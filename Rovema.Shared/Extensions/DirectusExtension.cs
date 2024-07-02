using Dclt.Directus;
using Rovema.Shared.Models;

namespace Rovema.Shared.Extensions;

public static class DirectusExtension
{
    public static async Task<IEnumerable<SolarPanelModel>?> GetPanels(this DirectusService service, int rpaId)
    {
        var query = new DirectusQuery()
            .Filter("rpa_id", Operation.Equal, rpaId)
            .Filter("status", Operation.Equal, "published")
            .Build();
        return await service.GetItemsAsync<IEnumerable<SolarPanelModel>>("solar_panels", query);
    }

    public static async Task<ReadWeatherModel?> GetWeather(this DirectusService service, int? rpaId)
    {
        if (rpaId == null) return null;
        var query = new DirectusQuery()
            .Filter("rpa_id", Operation.Equal, rpaId)
            .Sort("-date_created")
        .Limit(1)
            .Build();
        var weather = await service.GetItemsAsync<IEnumerable<ReadWeatherModel>>("reads_weather", query);
        return weather?.First();
    }
}
