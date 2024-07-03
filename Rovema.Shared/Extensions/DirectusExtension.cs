using Dclt.Directus;
using Rovema.Shared.Contracts;
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

    public static async Task<T?> GetCache<T>(this DirectusService service, long rpaId)
    {
        var cache = await service.GetItemAsync<CacheModel<T>>("cache", rpaId);
        if (cache != null && cache.Data != null)
            return cache.Data;
        return default;
    }
}
