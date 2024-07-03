using System.Text.Json.Serialization;

namespace Rovema.Shared.Models;

public class CacheModel<T>
{
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }
}

public class CacheModel
{
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }
}
