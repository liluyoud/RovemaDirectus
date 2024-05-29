using System.Text.Json.Serialization;

namespace Rovema.Shared.Contracts;

public class CreateReadWeather
{
    [JsonPropertyName("rpa_id")]
    public int RpaId { get; set; }

    [JsonPropertyName("weather_id")]
    public int WeatherId { get; set; }

    public string? Description { get; set; }

    public string? Icon { get; set; }

    [JsonPropertyName("temp_c")]
    public double? TempC { get; set; }

    [JsonPropertyName("feels_c")]
    public double? FeelsC { get; set; }

    public int? Humidity { get; set; }

    public int? Clouds { get; set; }
    
    [JsonPropertyName("wind_speed")]
    public double? WindSpeed { get; set; }

    [JsonPropertyName("wind_direction")]
    public int? WindDirection { get; set; }

    public int? Visibility { get; set; }
}
