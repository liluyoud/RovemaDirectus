using Dclt.Shared.Models;
using Refit;
using Rovema.Shared.Contracts;
using Rovema.Shared.Models;

namespace Rovema.Shared.Interfaces;

public interface IRovemaService
{
    [Get("/cache/weather/{rpaId}")]
    Task<ReadWeatherModel> GetCachedRpaWeatherAsync(int rpaId);


    [Get("/rpas/weather")]
    Task<IEnumerable<RpaModel>> GetRpasWeatherAsync();

    [Get("/rpas/ion")]
    Task<IEnumerable<RpaModel>> GetRpasIonAsync();

    [Get("/rpas/solar")]
    Task<IEnumerable<RpaModel>> GetRpasSolarAsync();

    [Get("/rpas/inverter")]
    Task<IEnumerable<RpaModel>> GetRpasInverterAsync();

    
    [Post("/reads/weather")]
    Task<ReadWeatherModel> AddWeatherAsync(CreateReadWeather read);

    [Post("/reads/ion")]
    Task<ReadIonModel> AddIonAsync(CreateReadIon read);

    [Post("/reads/inverter")]
    Task<ReadInverterModel> AddInverterAsync(CreateReadInverter read);

    [Post("/reads/solar")]
    Task<ReadIonModel> AddSolarAsync(CreateReadSolar read);

}
