using System.Text.Json.Serialization;

namespace Rovema.Shared.Contracts;

public class CreateReadWeather
{
    [JsonPropertyName("rpa_id")]
    public int RpaId { get; set; }

    [JsonPropertyName("weather_id")]
    public int WeatherId { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("temp_c")]
    public double? TempC { get; set; }

    [JsonPropertyName("feels_c")]
    public double? FeelsC { get; set; }

    [JsonPropertyName("humidity")]
    public int? Humidity { get; set; }

    [JsonPropertyName("clouds")]
    public int? Clouds { get; set; }
    
    [JsonPropertyName("wind_speed")]
    public double? WindSpeed { get; set; }

    [JsonPropertyName("wind_direction")]
    public int? WindDirection { get; set; }

    [JsonPropertyName("visibility")]
    public int? Visibility { get; set; }
}
