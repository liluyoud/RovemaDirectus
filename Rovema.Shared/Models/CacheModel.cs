using System.Text.Json.Serialization;

namespace Rovema.Shared.Contracts;

public class CacheModel<T>
{
    [JsonPropertyName("id")]
    public long? Id { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [JsonPropertyName("data")]
    public T? Data { get; set; }
}
