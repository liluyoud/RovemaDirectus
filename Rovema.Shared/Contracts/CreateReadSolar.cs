using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Rovema.Shared.Models;

public class CreateReadSolar
{
    [JsonPropertyName("rpa_id")]
    public int RpaId { get; set; }

    [JsonPropertyName("weather_id")]
    public int? WeatherId { get; set; }

    [JsonPropertyName("pir1")]
    public double? Pir1 { get; set; }
    [JsonPropertyName("pir2")]
    public double? Pir2 { get; set; }
    [JsonPropertyName("pir3")]
    public double? Pir3 { get; set; }
    [JsonPropertyName("pir4")]
    public double? Pir4 { get; set; }

    [JsonPropertyName("power1")]
    public double? Power1 { get; set; }
    [JsonPropertyName("power2")]
    public double? Power2 { get; set; }
    [JsonPropertyName("power3")]
    public double? Power3 { get; set; }
    [JsonPropertyName("power4")]
    public double? Power4 { get; set; }

    [JsonPropertyName("temp_c")]
    public double? TempC { get; set; }

    [JsonPropertyName("temp_air")]
    public double? TempAir { get; set; }

    [JsonPropertyName("temp_module")]
    public double? TempModule { get; set; }

    [JsonPropertyName("temp_internal")]
    public double? TempInternal { get; set; }

    [JsonPropertyName("humidity")]
    public double? Humidity { get; set; }

    [JsonPropertyName("wind_speed")]
    public double? WindSpeed { get; set; }

}
