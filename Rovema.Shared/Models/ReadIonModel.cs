using System.Text.Json.Serialization;

namespace Rovema.Shared.Models;

public class ReadIonModel
{
    public long Id { get; set; }

    [JsonPropertyName("rpa_id")]
    public int? RpaId { get; set; }

    [JsonPropertyName("date_created")]
    public DateTime? ReadAt { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }


    [JsonPropertyName("power_kw_total")]
    public double? Power_KW_Total { get; set; }

    [JsonPropertyName("power_kw_a")]
    public double? Power_KW_A { get; set; }

    [JsonPropertyName("power_kw_b")]
    public double? Power_KW_B { get; set; }

    [JsonPropertyName("power_kw_c")]
    public double? Power_KW_C { get; set; }


    [JsonPropertyName("power_kva_total")]
    public double? Power_KVA_Total { get; set; }

    [JsonPropertyName("power_kva_a")]
    public double? Power_KVA_A { get; set; }

    [JsonPropertyName("power_kva_b")]
    public double? Power_KVA_B { get; set; }

    [JsonPropertyName("power_kva_c")]
    public double? Power_KVA_C { get; set; }


    [JsonPropertyName("power_kvar_total")]
    public double? Power_KVAR_Total { get; set; }

    [JsonPropertyName("power_kvar_a")]
    public double? Power_KVAR_A { get; set; }

    [JsonPropertyName("power_kvar_b")]
    public double? Power_KVAR_B { get; set; }

    [JsonPropertyName("power_kvar_c")]
    public double? Power_KVAR_C { get; set; }


    [JsonPropertyName("current_i_avg")]
    public double? Current_I_AVG { get; set; }

    [JsonPropertyName("current_i_a")]
    public double? Current_I_A { get; set; }

    [JsonPropertyName("current_i_b")]
    public double? Current_I_B { get; set; }

    [JsonPropertyName("current_i_c")]
    public double? Current_I_C { get; set; }

    [JsonPropertyName("current_i_unbal")]
    public double? Current_I_Unbal { get; set; }


    [JsonPropertyName("factor_sign_total")]
    public double? Factor_Sign_Total { get; set; }

    [JsonPropertyName("factor_sign_a")]
    public double? Factor_Sign_A { get; set; }

    [JsonPropertyName("factor_sign_b")]
    public double? Factor_Sign_B { get; set; }

    [JsonPropertyName("factor_sign_c")]
    public double? Factor_Sign_C { get; set; }


    [JsonPropertyName("voltage_vln_avg")]
    public double? Voltage_Vln_Avg { get; set; }

    [JsonPropertyName("voltage_vln_a")]
    public double? Voltage_Vln_A { get; set; }

    [JsonPropertyName("voltage_vln_b")]
    public double? Voltage_Vln_B { get; set; }

    [JsonPropertyName("voltage_vln_c")]
    public double? Voltage_Vln_C { get; set; }


    [JsonPropertyName("voltage_vll_avg")]
    public double? Voltage_Vll_Avg { get; set; }

    [JsonPropertyName("voltage_vll_ab")]
    public double? Voltage_Vll_AB { get; set; }

    [JsonPropertyName("voltage_vll_bc")]
    public double? Voltage_Vll_BC { get; set; }

    [JsonPropertyName("voltage_vll_ca")]
    public double? Voltage_Vll_CA { get; set; }

    [JsonPropertyName("voltage_v_unbal")]
    public double? Voltage_V_Unbal { get; set; }


    [JsonPropertyName("frequency")]
    public double? Frequency { get; set; }
}
