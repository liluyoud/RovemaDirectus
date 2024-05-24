using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Dclt.Shared.Extensions;

namespace Rovema.Shared.Services;

public class RovemaService
{
    private readonly ILogger<RovemaService> _logger;
    private readonly IConfiguration _conf;
    private readonly IDistributedCache _cache;
    private readonly IHttpClientFactory _http;
    private readonly HttpClient _client;

    public RovemaService(ILogger<RovemaService> logger, IConfiguration conf, IDistributedCache cache, IHttpClientFactory http)
    {
        _logger = logger;
        _conf = conf;
        _cache = cache;
        _http = http;
        
        var baseUrl = Environment.GetEnvironmentVariable("DIRECTUS_URL") ?? conf["Environment:DIRECTUS_URL"] ?? "";
        var accessToken = Environment.GetEnvironmentVariable("DIRECTUS_TOKEN") ?? conf["Environment:DIRECTUS_TOKEN"] ?? "";
        _client = _http.CreateClient();
        _client.BaseAddress = new Uri(baseUrl);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
    }

    public async Task<TData?> GetCachedItemAsync<TData>(string collection, long id ) where TData : class
    {
        var item = await _cache.GetAsync($"{nameof(TData)}-{id}", async token => {

            return await GetItemAsync<TData>(collection, id);
        }, CacheOptions.FiveMinutesExpiration);
        return item;
        
    }

    public async Task<TData?> GetItemAsync<TData>(string collection, long id) where TData : class
    {
        var response = await _client.GetAsync($"/items/{collection}/{id.ToString()}");
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(content))
            {
                var item = JsonSerializer.Deserialize<ResponseBaseModel<TData>>(content, new JsonSerializerOptions { 
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, 
                    AllowTrailingCommas = true, 
                    PropertyNameCaseInsensitive = true 
                });
                return item?.Data;
            }
        }
        return default;
    }

    //public async Task<List<RpaModel>?> GetRpasAsync(string? type = null)
    //{
    //    string? filter = null;
    //    if (type != null) filter = string.Concat("{ \"_and\": [ { \"type\" : { \"_eq\" :\"", type, "\" } }, { \"status\" : { \"_eq\" : \"published\" } } ] }");  
    //    var rpas = await _directus.GetRpasAsync(_accessToken, "id,name,type,status,settings", filter);
    //    _logger.LogInformation("GetRpasAsync executed");
    //    return rpas.Data;
    //}


}

public class ResponseBaseModel<TData> where TData : class 
{
    public long Id { get; set; }
    public TData? Data { get; set; }
}