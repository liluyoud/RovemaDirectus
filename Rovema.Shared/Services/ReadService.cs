using Microsoft.Extensions.Logging;
using HtmlAgilityPack;
using System.Net;
using Dclt.Shared.Models;
using Dclt.Shared.Extensions;
using Microsoft.Extensions.Caching.Distributed;

namespace Rovema.Shared.Services;

public class ReadService
{
    private readonly ILogger<ReadService> _logger;
    private readonly IDistributedCache _cache;


    public ReadService(ILogger<ReadService> logger, IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task<List<KeyValueModel>?> GetCachedSolarAsync(string address)
    {
        var item = await _cache.GetAsync($"solar-{address}", async token => {

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
            _logger.LogError($"GetSolarAsync {address} not executed: {ex.Message}");
        }
        return default;
    }

}

