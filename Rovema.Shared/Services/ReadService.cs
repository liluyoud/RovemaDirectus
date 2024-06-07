using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using System.Net;
using Dclt.Shared.Models;
using Dclt.Shared.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Rovema.Shared.Models;
using Dclt.Shared.Services;

namespace Rovema.Shared.Services;

public class ReadService(ILogger<ReadService> logger, IDistributedCache cache, DirectusService directus)
{
    public async Task<List<KeyValueModel>?> GetCachedSolarAsync(string address)
    {
        var item = await cache.GetAsync($"solarimetric-{address}", async token => {

            return await GetSolarAsync(address);
        }, CacheOptions.GetExpiration(1));
        return item;
    }

    public async Task<List<KeyValueModel>?> GetSolarAsync(string address)
    {
        try
        {
            var url = $"http://{address}/channels.php";
            var web = new HtmlWeb();
            web.PreRequest = delegate (HttpWebRequest webRequest)
            {
                webRequest.Timeout = 20000;
                return true;
            };

            var html = await web.LoadFromWebAsync(url);
            var table = html.DocumentNode.SelectSingleNode("//table");
            var rows = table.SelectNodes("//tr").Skip(1);
            List<KeyValueModel> values = new List<KeyValueModel>();

            foreach (var row in rows)
            {
                var cells = row.SelectNodes("td");
                KeyValueModel value = new(cells[1].InnerText.Trim(), cells[2].InnerText.Trim());
                values.Add(value);
            }
            return values;
        }
        catch (Exception ex)
        {
            logger.LogError($"GetSolarAsync {address} not executed: {ex.Message}");
        }
        return default;
    }

    public async Task<IEnumerable<SolarPanelModel>?> GetCachedSolarPanelsAsync(int rpaId)
    {
        var item = await cache.GetAsync($"solarpanels-{rpaId}", async token => {

            return await GetSolarPanelsAsync(rpaId);
        }, CacheOptions.GetExpiration(10));
        return item;
    }

    public async Task<IEnumerable<SolarPanelModel>?> GetSolarPanelsAsync(int rpaId)
    {
        var filter = $"{{ \"_and\": [ {{ \"rpa_id\": {{ \"_eq\": \"{rpaId}\" }} }}, {{ \"status\": {{ \"_eq\": \"published\" }} }} ] }}";
        var panels = await directus.GetCachedItemsAsync<IEnumerable<SolarPanelModel>>($"panels-{rpaId}", "solar_panels", 30, filter);
        return panels;
    }

}

