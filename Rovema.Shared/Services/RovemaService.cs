using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Rovema.Shared.Interfaces;
using Rovema.Shared.Models;
namespace Rovema.Shared.Services;

public class RovemaService
{
    private readonly ILogger<RovemaService> _logger;
    private readonly IConfiguration _conf;
    private readonly IDirectusService _directus;
    private readonly string _accessToken;

    public RovemaService(ILogger<RovemaService> logger, IConfiguration conf, IDirectusService directus)
    {
        _logger = logger;
        _conf = conf;
        _directus = directus;
        _accessToken = Environment.GetEnvironmentVariable("DIRECTUS_TOKEN") ?? _conf["Environment:DIRECTUS_TOKEN"] ?? "";
    }

    public async Task<List<RpaModel>?> GetRpasAsync(string? type = null)
    {
        string? filter = null;
        if (type != null) filter = string.Concat("{ \"_and\": [ { \"type\" : { \"_eq\" :\"", type, "\" } }, { \"status\" : { \"_eq\" : \"published\" } } ] }");  
        var rpas = await _directus.GetRpasAsync(_accessToken, "id,name,type,status,settings", filter);
        _logger.LogInformation("GetRpasAsync executed");
        return rpas.Data;
    }

    
}
