using System.Text.Json.Serialization;

namespace Rovema.Shared.Models;

public class SolarPanelModel
{
    public long Id { get; set; }

    [JsonPropertyName("rpa_id")]
    public int RpaId { get; set; }

    [JsonPropertyName("brand")]
    public string? Brand { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("power")]
    public double Power { get; set; }

    [JsonPropertyName("tnoc")]
    public double Tnoc { get; set; }

    [JsonPropertyName("ymp")]
    public double Ymp { get; set; }

    [JsonPropertyName("tc0")]
    public double Tc0 { get; set; }

    [JsonPropertyName("ratio")]
    public double Ratio { get; set; }

}
