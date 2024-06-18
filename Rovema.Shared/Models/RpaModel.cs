using Dclt.Shared.Models;

namespace Rovema.Shared.Models;

public record RpaModel(int Id, string Name, string Type, List<KeyValue>? Settings);

