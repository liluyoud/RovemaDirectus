using Dclt.Shared.Models;
using Rovema.Shared.Models;

namespace Rovema.Shared.Contracts;

public record CreateRpa(string Name, string Type, List<KeyValueModel>? Settings);