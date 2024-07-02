using System.Text.Json.Serialization;

namespace Rovema.Shared.Contracts;

public class CacheModel<T>
{
    [JsonPropertyName("id")]
    public double? Id { get; set; }

    [JsonPropertyName("read_at")]
    public double? ReadAt { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }
}
