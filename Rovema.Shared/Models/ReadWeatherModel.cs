using System.Text.Json.Serialization;

namespace Rovema.Shared.Models;

public class ReadWeatherModel
{
    public long Id { get; set; }

    [JsonPropertyName("rpa_id")]
    public int RpaId { get; set; }

    [JsonPropertyName("date_created")]
    public DateTime ReadAt { get; set; }

    [JsonPropertyName("weather_id")]
    public int WeatherId { get; set; }

    public string? Description { get; set; }

    public string? Icon { get; set; }

    [JsonPropertyName("temp_c")]
    public double? TempC { get; set; }

    [JsonPropertyName("feels_c")]
    public double? FeelsC { get; set; }

    public double? Humidity { get; set; }

    public double? Clouds { get; set; }
    
    [JsonPropertyName("wind_speed")]
    public double? WindSpeed { get; set; }

    [JsonPropertyName("wind_direction")]
    public double? WindDirection { get; set; }

    public double? Visibility { get; set; }
}
