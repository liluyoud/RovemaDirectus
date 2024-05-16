using Refit;
using Rovema.Shared.Models;

namespace Rovema.Shared.Interfaces;

public interface IRovemaService
{
    [Get("/weather")]
    Task<ReadWeatherModel> GetWeatherAsync(int id, string latitude, string longitude);

    [Get("/rpas")]
    Task<List<RpaModel>> GetRpasAsync();

    [Get("/rpas/{type}")]
    Task<List<RpaModel>> GetRpasAsync(string type);
}
