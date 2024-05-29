using Dclt.Shared.Models;
using Refit;
using Rovema.Shared.Contracts;
using Rovema.Shared.Models;

namespace Rovema.Shared.Interfaces;

public interface IRovemaService
{
    [Get("/rpas/weather")]
    Task<IEnumerable<RpaModel>> GetRpasWeatherAsync();

    [Get("/weather")]
    Task<WeatherModel> GetWeatherAsync(string latitude, string longitude);

    [Post("/reads/weather")]
    Task<ReadWeatherModel> AddWeatherAsync(CreateReadWeather read);

}
