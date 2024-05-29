using Dclt.Shared.Enums;
using Dclt.Shared.Extensions;
using Dclt.Shared.Models;
using Rovema.Shared.Contracts;
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
            Description = weather.Description?.PrimeiraMaiuscula(),
            Icon = weather.ToDirectusIcon(),
            TempC = weather.TempC,
            FeelsC = weather.FeelsC,
            Humidity = (int?)weather.Humidity,
            Clouds = (int?)weather.Clouds,
            WindSpeed = weather.WindSpeed,    
            WindDirection = (int?)weather.WindDirection,
            Visibility = (int?)weather.Visibility
        };
    }

    public static CreateReadWeather ToCreateReadModel(this WeatherModel weather, int rpaId)
    {
        return new CreateReadWeather
        {
            RpaId = rpaId,
            WeatherId = weather.WeatherId,
            Description = weather.Description?.PrimeiraMaiuscula(),
            Icon = weather.ToDirectusIcon(),
            TempC = weather.TempC,
            FeelsC = weather.FeelsC,
            Humidity = (int?)weather.Humidity,
            Clouds = (int?)weather.Clouds,
            WindSpeed = weather.WindSpeed,
            WindDirection = (int?)weather.WindDirection,
            Visibility = (int?)weather.Visibility
        };
    }

    public static string ToDirectusIcon(this WeatherModel weather)
    {
        var day = weather.ReadAt.IsDayOrNight(weather.Sunrise, weather.Sunset) == DayNight.Day;
        var icon = day ? "sunny" : "clear_night";
        if (weather.Description == "algumas nuvens")
            icon = day ? "partly_cloudy_day" : "partly_cloudy_night";
        else if (weather.Description == "nuvens dispersas")
            icon = "cloud";
        else if (weather.Description == "nublado")
            icon = "foggy";
        else if (weather.Description == "chuviscos")
            icon = "cloudy_snowing";
        else if (weather.Description == "garoa de leve intensidade")
            icon = "cloudy_snowing";
        else if (weather.Description == "chuvas")
            icon = "rainy";
        else if (weather.Description == "chuva forte")
            icon = "rainy";
        else if (weather.Description == "chuva leve")
            icon = "cloudy_snowing";
        else if (weather.Description == "chuva moderada")
            icon = "rainy";
        else if (weather.Description == "chuva muito forte")
            icon = "rainy";
        else if (weather.Description == "trovoada com chuva")
            icon = "thunderstorm";
        else if (weather.Description == "trovoada com chuva forte")
            icon = "thunderstorm";
        else if (weather.Description == "trovoada com chuva fraca")
            icon = "thunderstorm";
        else if (weather.Description == "trovoadas")
            icon = "thunderstorm";
        else if (weather.Description == "névoa")
            icon = "grain";
        else if (weather.Description == "neblina")
            icon = "grain";
        else if (weather.Description == "fumaça")
            icon = "grain";
        return icon;
    }
}
