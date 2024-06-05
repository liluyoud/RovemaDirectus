using System.Text.Json.Serialization;

namespace Rovema.Shared.Models;

public class ReadSolarModel
{
    public long Id { get; set; }

    [JsonPropertyName("rpa_id")]
    public int RpaId { get; set; }

    [JsonPropertyName("date_created")]
    public DateTime ReadAt { get; set; }

    public double? Pir1 { get; set; }
    public double? Pir2 { get; set; }
    public double? Pir3 { get; set; }
    public double? Pir4 { get; set; }

    public double? Power1 { get; set; }
    public double? Power2 { get; set; }
    public double? Power3 { get; set; }
    public double? Power4 { get; set; }

    [JsonPropertyName("temp_air")]
    public double? TempAir { get; set; }

    [JsonPropertyName("temp_module")]
    public double? TempModule { get; set; }

    [JsonPropertyName("temp_internal")]
    public double? TempInternal { get; set; }

    public double? Humidity { get; set; }

    [JsonPropertyName("wind_speed")]
    public double? WindSpeed { get; set; }

}
