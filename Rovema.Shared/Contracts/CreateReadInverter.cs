using System.Text.Json.Serialization;

namespace Rovema.Shared.Models;

public class CreateReadInverter
{
    [JsonPropertyName("rpa_id")]
    public int RpaId { get; set; }
    public double? Current { get; set; }
    public double? Day { get; set; }
    public double? Month { get; set; }
    public double? Year { get; set; }
    public double? Lifetime { get; set; }
}
