using Dclt.Shared.Models;
using Rovema.Shared.Models;

namespace Rovema.Shared.Extensions;

public static class WeatherExtension
{
    public static ReadWeatherModel ToReadModel(this WeatherModel weather, int rpaId)
    {
        return new ReadWeatherModel
        {
            RpaId = rpaId,
            ReadAt = weather.ReadAt,  
            WeatherId = weather.WeatherId,
            Description = weather.Description,
            Icon = weather.Icon,
            Sunrise = weather.Sunrise,
            Sunset = weather.Sunset,
            TempC = weather.TempC,
            FeelsC = weather.FeelsC,
            Humidity = (int?)weather.Humidity,
            Clouds = (int?)weather.Clouds,
            WindSpeed = weather.WindSpeed,    
            WindDirection = (int?)weather.WindDirection,
            Visibility = (int?)weather.Visibility
        };
    }
}
