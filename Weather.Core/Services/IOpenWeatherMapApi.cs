using System.Threading;
using System.Threading.Tasks;
using Refit;

namespace Weather.Core.Services
{
    public interface IOpenWeatherMapApi
    {
        [Get("/data/2.5/weather?q={cityname},{countrycode}")]
        Task<ApiResponse<CurrentWeatherApiResponse>> GetCurrentWeatherAsync(string cityName, string countryCode, CancellationToken cancellationToken = default);
    }
}
