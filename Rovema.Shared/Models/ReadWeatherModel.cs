namespace Rovema.Shared.Models;

public class ReadWeatherModel
{
    public long Id { get; set; }
    public long RpaId { get; set; }
    public DateTime ReadAt { get; set; }
    public int? WeatherId { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public DateTime? Sunrise { get; set; }
    public DateTime? Sunset { get; set; }
    public double? TempC { get; set; }
    public double? FeelsC { get; set; }
    public int? Humidity { get; set; }
    public int? Clouds { get; set; }
    public double? WindSpeed { get; set; }
    public int? WindDirection { get; set; }
    public int? Visibility { get; set; }
}
