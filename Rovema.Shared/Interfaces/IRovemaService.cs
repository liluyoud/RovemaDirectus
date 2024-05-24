using Dclt.Shared.Models;
using Refit;
using Rovema.Shared.Models;

namespace Rovema.Shared.Interfaces;

public interface IRovemaService
{
    [Get("/items/rpas")]
    Task<ResponseModel<RpaModel>> GetRpasAsync();
    //Task<ResponseModel<RpaModel>> GetRpasAsync(string access_token, string? fields, string? filter);

    [Get("/items/rpas/{id}")]
    Task<ResponseModel<RpaModel>> GetRpaByIdAsync(long id, string access_token, string? fields);

    [Post("/items/rpas")]
    Task PostRpasAsync([Query] string access_token, [Body] IEnumerable<ReadWeatherModel> reads);
}
