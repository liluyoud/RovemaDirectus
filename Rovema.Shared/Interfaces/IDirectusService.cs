using Refit;
using Rovema.Shared.Models;

namespace Rovema.Shared.Interfaces;

public interface IDirectusService
{
    [Get("/items/rpas")]
    Task<RpaResponse> GetRpasAsync(string access_token, string? fields, string? filter);
}
