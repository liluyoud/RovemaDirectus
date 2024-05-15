using Refit;
using Rovema.Shared.Models;

namespace Rovema.Shared.Interfaces;

public interface IOpenWeatherService
{
    [Get("/data/2.5/weather?lat={latitude}&lon={longitude}&units=metric&lang=pt_br&appid={apiKey}")]
    Task<OpenWeatherModel> GetWeatherAsync(string latitude, string longitude, string apiKey);
}
