using Dclt.Shared.Models;

namespace Rovema.Shared.Contracts;

public record CreateRpa(string Name, string Type, List<KeyValue>? Settings);