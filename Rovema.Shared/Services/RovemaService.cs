using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rovema.Shared.Extensions;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Models;
using System.Text.Json;
namespace Rovema.Shared.Services;

public class RovemaService
{
    private readonly ILogger<RovemaService> _logger;
    private readonly IConfiguration _conf;
    private readonly IDistributedCache _cache;
    private readonly IDirectusService _directus;
    private readonly IOpenWeatherService _openWeather;
    private readonly string _accessToken;
    private readonly string _openWeatherKey;

    public RovemaService(ILogger<RovemaService> logger, IConfiguration conf, IDistributedCache cache, IDirectusService directus, IOpenWeatherService openWeather)
    {
        _logger = logger;
        _conf = conf;
        _cache = cache;
        _directus = directus;
        _openWeather = openWeather;
        _accessToken = Environment.GetEnvironmentVariable("DIRECTUS_TOKEN") ?? _conf["Environment:DIRECTUS_TOKEN"] ?? "";
        _openWeatherKey = Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY") ?? _conf["Environment:OPENWEATHER_API_KEY"] ?? "";
    }

    public async Task<List<RpaModel>?> GetRpasAsync(string? type = null)
    {
        string? filter = null;
        if (type != null) filter = string.Concat("{ \"_and\": [ { \"type\" : { \"_eq\" :\"", type, "\" } }, { \"status\" : { \"_eq\" : \"published\" } } ] }");  
        var rpas = await _directus.GetRpasAsync(_accessToken, "id,name,type,status,settings", filter);
        _logger.LogInformation("GetRpasAsync executed");
        return rpas.Data;
    }

    public async Task<OpenWeatherModel?> GetWeatherAsync(string? latitude, string? longitude)
    {
        if (latitude != null && longitude != null)
        {
            var weather = await _openWeather.GetWeatherAsync(latitude, longitude, _openWeatherKey);
            _logger.LogInformation("GetWeatherAsync executed");
            return weather;
        }
        return null;
    }

    public async Task<ReadWeatherModel?> GetReadWeatherAsync(RpaModel? rpa)
    {
        if (rpa != null && rpa.Type == "Tempo")
        {
            var latitude = rpa.GetSetting("Latitude");
            var longitude = rpa.GetSetting("Longitude");
            var weather = await GetWeatherAsync(latitude, longitude);
            var read = weather.GetRead();
            if (read != null)
            {
                read.RpaId = rpa.Id;
                read.ReadAt = DateTime.UtcNow;
                string readJson = JsonSerializer.Serialize(read);
                await _cache.SetStringAsync("rpaWeather-" + rpa.Id.ToString(), readJson);
                _logger.LogInformation("GetReadWeatherAsync executed");
                return read;
            }
        }
        return null;
    }

    public async Task<List<ReadWeatherModel>?> GetReadWeathersAsync()
    {
        var rpas = await GetRpasAsync("Tempo");
        if (rpas != null)
        {
            var reads = new List<ReadWeatherModel>();
            foreach (var rpa in rpas)
            {
                var read = await GetReadWeatherAsync(rpa);
                if (read != null)
                    reads.Add(read);
            }
            _logger.LogInformation("GetReadWeathersAsync executed");
            return reads;
        }
        return null;
    }
}
