using Dclt.Shared.Models;
using System.Text.Json.Serialization;

namespace Rovema.Shared.Models;

public record PowerPlantModel {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public double? Power { get; set; }

    [JsonPropertyName("ion_id")]
    public int? IonId { get; set; }

    [JsonPropertyName("inverter_id")]
    public int? InverterId { get; set; }

    [JsonPropertyName("solar_id")]
    public int? SolarId { get; set; }

    [JsonPropertyName("weather_id")]
    public int? WeatherId { get; set; }
}
