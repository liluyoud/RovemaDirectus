﻿using System.Text.Json.Serialization;

namespace Rovema.Shared.Models;

public class ReadInverterModel
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("rpa_id")]
    public int RpaId { get; set; }

    [JsonPropertyName("date_created")]
    public DateTime ReadAt { get; set; }

    [JsonPropertyName("current")]
    public double? Current { get; set; }

    [JsonPropertyName("day")]
    public double? Day { get; set; }

    [JsonPropertyName("month")]
    public double? Month { get; set; }

    [JsonPropertyName("year")]
    public double? Year { get; set; }

    [JsonPropertyName("lifetime")]
    public double? Lifetime { get; set; }
}
