namespace Rovema.Shared.Models;

public record PowerPlantSumaryModel {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public double Power { get; set; }
    public int IonId { get; set; }
    public bool IonOk { get; set; }
    public double IonKw { get; set; }
    public double IonDay { get; set; }
    public double IonMonth { get; set; }
    public double IonDayConsume { get; set; }
    public double IonMonthConsume { get; set; }
    public double IonCurrentA { get; set; }
    public double IonTensionV { get; set; }
    public double IonKVar { get; set; }
    public double IonFrequency { get; set; }
    public int InverterId { get; set; }
    public bool InverterOk { get; set; }
    public double InverterKw { get; set; }
    public double InverterDay { get; set; }
    public double InverterMonth { get; set; }
    public int SolarId { get; set; }
    public bool SolarOk { get; set; }
    public double SolarKw { get; set; }
    public int WeatherId { get; set; }
    public bool WeatherOk { get; set; }
    public double WeatherTempC { get; set; }
    public string? WeatherDescrition { get; set; }
    public double WeatherClouds { get; set; }
}
