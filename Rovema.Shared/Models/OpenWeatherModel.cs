using System.Text.Json.Serialization;

namespace Rovema.Shared.Models;

public class OpenWeatherModel
{
    public CoordModel? Coord { get; set; }
    public List<WeatherModel>? Weather { get; set; }
    public string? Base { get; set; }
    public MainModel? Main { get; set; }
    public int? Visibility { get; set; }
    public WindModel? Wind { get; set; }
    public CloudsModel? Clouds { get; set; }
    public RainModel? Rain { get; set; }
    public int? Dt { get; set; }
    public SysModel? Sys { get; set; }
    public int? Timezone { get; set; }
    public int? Id { get; set; }
    public string? Name { get; set; }
    public int? Cod { get; set; }

    public class CoordModel
    {
        public double? Lon { get; set; }
        public double? Lat { get; set; }
    }

    public class WeatherModel
    {
        public int? Id { get; set; }
        public string? Main { get; set; }
        public string? Description { get; set; }
        public string? Icon { get; set; }
    }

    public class MainModel
    {
        public double? Temp { get; set; }
        public double? Feels_like { get; set; }
        public double? Temp_min { get; set; }
        public double? Temp_max { get; set; }
        public int? Pressure { get; set; }
        public int? Humidity { get; set; }
        public int? Sea_level { get; set; }
        public int? Grnd_level { get; set; }
    }

    public class WindModel
    {
        public double? Speed { get; set; }
        public int? Deg { get; set; }
    }

    public class CloudsModel
    {
        public int? All { get; set; }
    }

    public class RainModel
    {
        [JsonPropertyName("1h")]
        public double? _1h { get; set; }
    }

    public class SysModel
    {
        public string? Country { get; set; }
        public long? Sunrise { get; set; }
        public long? Sunset { get; set; }
    }
}

