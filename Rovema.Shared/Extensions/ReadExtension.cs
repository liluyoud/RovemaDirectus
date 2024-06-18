using Dclt.Services.OpenWeather;
using Dclt.Shared.Enums;
using Dclt.Shared.Extensions;
using Dclt.Shared.Models;
using Rovema.Shared.Contracts;
using Rovema.Shared.Models;

namespace Rovema.Shared.Extensions;

public static class ReadExtension
{
    public static CreateReadIon ToCreateReadIon (this IonModel ion, int rpaId, string Type)
    {
        return new CreateReadIon()
        {
            RpaId = rpaId,
            Type = Type,
            Power_KW_Total = ion.Power_KW_Total,
            Power_KW_A = ion.Power_KW_A,
            Power_KW_B = ion.Power_KW_B,
            Power_KW_C = ion.Power_KW_C,
            Power_KVA_Total = ion.Power_KVA_Total,
            Power_KVA_A = ion.Power_KVA_A,
            Power_KVA_B = ion.Power_KVA_B,
            Power_KVA_C = ion.Power_KVA_C,
            Power_KVAR_Total = ion.Power_KVAR_Total,
            Power_KVAR_A = ion.Power_KVAR_A,
            Power_KVAR_B = ion.Power_KVAR_B,
            Power_KVAR_C = ion.Power_KVAR_C,
            Current_I_AVG = ion.Current_I_AVG,
            Current_I_A = ion.Current_I_A,
            Current_I_B = ion.Current_I_B,
            Current_I_C = ion.Current_I_C,
            Current_I_Unbal = ion.Current_I_Unbal,
            Factor_Sign_Total = ion.Factor_Sign_Total,
            Factor_Sign_A = ion.Factor_Sign_A,
            Factor_Sign_B = ion.Factor_Sign_B,
            Factor_Sign_C = ion.Factor_Sign_C,
            Voltage_Vln_Avg = ion.Voltage_Vln_Avg,
            Voltage_Vln_A = ion.Voltage_Vln_A,
            Voltage_Vln_B = ion.Voltage_Vln_B,
            Voltage_Vln_C = ion.Voltage_Vln_C,
            Voltage_Vll_Avg = ion.Voltage_Vll_Avg,
            Voltage_Vll_AB = ion.Voltage_Vll_AB,
            Voltage_Vll_BC = ion.Voltage_Vll_BC,
            Voltage_Vll_CA = ion.Voltage_Vll_CA,
            Voltage_V_Unbal = ion.Voltage_V_Unbal,
            Frequency = ion.Frequency
        };
    }

    public static CreateReadSolar ToCreateReadSolar(this List<KeyValue> model, RpaModel rpa)
    {
        var address = rpa.Settings.GetKey("address");
        var weatherId = rpa.Settings.GetKey("weatherId");
        var pir1 = rpa.Settings.GetKey("pir1");
        var pir2 = rpa.Settings.GetKey("pir2");
        var pir3 = rpa.Settings.GetKey("pir3");
        var pir4 = rpa.Settings.GetKey("pir4");
        var tempAir = rpa.Settings.GetKey("tempAir");
        var tempModule = rpa.Settings.GetKey("tempModule");
        var tempInternal = rpa.Settings.GetKey("tempInternal");
        var humidity = rpa.Settings.GetKey("humidity");
        var windSpeed = rpa.Settings.GetKey("windSpeed");

        pir1 = model.GetKey(pir1);
        pir2 = model.GetKey(pir2);
        pir3 = model.GetKey(pir3);
        pir4 = model.GetKey(pir4);
        tempAir = model.GetKey(tempAir);
        tempModule = model.GetKey(tempModule);
        tempInternal = model.GetKey(tempInternal);
        humidity = model.GetKey(humidity);
        windSpeed = model.GetKey(windSpeed);

        var read = new CreateReadSolar
        {
            RpaId = rpa.Id,
            WeatherId = weatherId != null ? int.Parse(weatherId) : null,
            Pir1 = pir1.ToDouble('.').Absolute(),
            Pir2 = pir2.ToDouble('.').Absolute(),
            Pir3 = pir3.ToDouble('.').Absolute(),
            Pir4 = pir4.ToDouble('.').Absolute(),
            TempAir = tempAir.ToDouble('.').Absolute(),
            TempModule = tempModule.ToDouble('.').Absolute(),
            TempInternal = tempInternal.ToDouble('.').Absolute(),
            Humidity = humidity.ToDouble('.').Absolute(),
            WindSpeed = windSpeed.ToDouble('.').Absolute()
        };

        return read;
    }

    public static CreateReadWeather ToCreateReadWeather(this Weather weather, int rpaId)
    {
        return new CreateReadWeather
        {
            RpaId = rpaId,
            WeatherId = weather.WeatherId,
            Description = weather.Description?.ToFirstUppercase(),
            Icon = weather.ToDirectusIcon(),
            TempC = weather.TempC,
            FeelsC = weather.FeelsC,
            Humidity = (int?)weather.Humidity,
            Clouds = (int?)weather.Clouds,
            WindSpeed = weather.WindSpeed,
            WindDirection = (int?)weather.WindDirection,
            Visibility = (int?)weather.Visibility
        };
    }

    public static void BuildTeoricPower(this CreateReadSolar read, IEnumerable<SolarPanelModel>? panels, ReadWeatherModel? weather)
    {
        if (panels != null)
        {
            read.TempC = weather?.TempC;

            var tempC = read.TempC ?? read.TempAir ?? 25;
            tempC = tempC > 50 ? tempC : 25;

            read.Power1 = read.Pir1.BuildTeoricPower(tempC, panels);
            read.Power2 = read.Pir2.BuildTeoricPower(tempC, panels);
            read.Power3 = read.Pir3.BuildTeoricPower(tempC, panels);
            read.Power4 = read.Pir4.BuildTeoricPower(tempC, panels);
        }
    }

    public static double? BuildTeoricPower(this double? pir, double tempC, IEnumerable<SolarPanelModel> panels)
    {
        if (pir == null || panels.Count() == 0) return null;

        double gt = pir.Value;
        var panelsArray = panels.ToArray();

        // calc TC
        var tc = new double[panelsArray.Count()];
        for (int i = 0; i < tc.Length; i++)
        {
            var tnoc = panelsArray[i].Tnoc;
            tc[i] = tempC + ((gt / 800) * (tnoc - 20) * 0.9);
        }

        // calc Pmp
        var pmp = new double[panelsArray.Count()];
        for (int i = 0; i < pmp.Length; i++)
        {
            var power = panelsArray[i].Power * panelsArray[i].Ratio;
            var ymp = panelsArray[i].Ymp;
            var tc0 = panelsArray[i].Tc0;
            pmp[i] = power * (gt / 1000) * (1 + (ymp * (tc[i] - tc0)));
        }

        // calc Teoric Power
        double teoricPower = 0;
        for (int i = 0; i < panelsArray.Count(); i++)
        {
            teoricPower += pmp[i] * panelsArray[i].Quantity;
        }

        return teoricPower / 1000;
    }

    public static string ToDirectusIcon(this Weather weather)
    {
        var day = weather.ReadAt.IsDayOrNight(weather.Sunrise, weather.Sunset) == DayNight.Day;
        var icon = day ? "sunny" : "clear_night";
        if (weather.Description == "algumas nuvens")
            icon = day ? "partly_cloudy_day" : "partly_cloudy_night";
        else if (weather.Description == "nuvens dispersas")
            icon = "cloud";
        else if (weather.Description == "nublado")
            icon = "foggy";
        else if (weather.Description == "chuviscos")
            icon = "cloudy_snowing";
        else if (weather.Description == "garoa de leve intensidade")
            icon = "cloudy_snowing";
        else if (weather.Description == "chuvas")
            icon = "rainy";
        else if (weather.Description == "chuva forte")
            icon = "rainy";
        else if (weather.Description == "chuva leve")
            icon = "cloudy_snowing";
        else if (weather.Description == "chuva moderada")
            icon = "rainy";
        else if (weather.Description == "chuva muito forte")
            icon = "rainy";
        else if (weather.Description == "trovoada com chuva")
            icon = "thunderstorm";
        else if (weather.Description == "trovoada com chuva forte")
            icon = "thunderstorm";
        else if (weather.Description == "trovoada com chuva fraca")
            icon = "thunderstorm";
        else if (weather.Description == "trovoadas")
            icon = "thunderstorm";
        else if (weather.Description == "névoa")
            icon = "grain";
        else if (weather.Description == "neblina")
            icon = "grain";
        else if (weather.Description == "fumaça")
            icon = "grain";
        return icon;
    }

    public static double GetPowerMultiplicator(this string? unit)
    {
        if (unit == null) return 1;
        if (unit.ToLower().Contains("mw"))
            return 1000;
        else if (unit.ToLower().Contains("gw"))
            return 1000000;
        else if (unit.ToLower().Contains("kw"))
            return 1;
        else if (unit.ToLower().Contains("w"))
            return 0.001;
        return 1;
    }
}
