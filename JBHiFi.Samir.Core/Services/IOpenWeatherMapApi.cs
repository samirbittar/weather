using System.Threading.Tasks;
using Refit;

namespace JBHiFi.Samir.Core.Services
{
    public interface IOpenWeatherMapApi
    {
        [Get("/data/2.5/weather?q={cityname},{countrycode}")]
        Task<ApiResponse<CurrentWeatherApiResponse>> GetCurrentWeather(string cityName, string countryCode);
    }
}
