namespace Rovema.Shared.Models;

public class ReadWeatherModel
{
    public int? RpaId { get; set; }
    public DateTime ReadAt { get; set; }
    public int? WeatherId { get; set; }
    public string? Text { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }

    public DateTime? Sunrise { get; set; }
    public DateTime? Sunset { get; set; }
    public double? TempC { get; set; }
    public double? FeelsC { get; set; }
    public double? TempMin { get; set; }
    public double? TempMax { get; set; }
    public double? PressureHpa { get; set; }
    public double? Humidity { get; set; }
    public double? SeaLevel { get; set; }
    public double? UV { get; set; }
    public double? GroundLevel { get; set; }

    public double? Clouds { get; set; }
    public double? WindSpeed { get; set; }
    public double? WindDirection { get; set; }
    public double? Visibility { get; set; }

    public double? Rain1h { get; set; }
}
