using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Weather.Core.Services
{
    public interface IOpenWeatherMapApiKeyProvider
    {
        public string GetApiKey();
    }

    public class RandomOpenWeatherMapApiKeyProvider : IOpenWeatherMapApiKeyProvider
    {
        private readonly IOptionsSnapshot<OpenWeatherMapApiOptions> _options;
        private readonly ILogger<RandomOpenWeatherMapApiKeyProvider> _logger;

        public RandomOpenWeatherMapApiKeyProvider(IOptionsSnapshot<OpenWeatherMapApiOptions> options, ILogger<RandomOpenWeatherMapApiKeyProvider> logger)
        {
            _options = options;
            _logger = logger;
        }

        public string GetApiKey()
        {
            if (_options.Value.ApiKeys == null || _options.Value.ApiKeys.Length == 0)
            {
                _logger.LogError("No OpenWeatherMap Api Keys were provided");
                throw new Exception("No OpenWeatherMap Api Keys were provided");
            }

            if (_options.Value.ApiKeys.Length == 1)
            {
                return _options.Value.ApiKeys[0];
            }

            // Randomising the api key that is returned will result in lower chances of us being rate limited by the
            // OpenWeatherMap API.
            var random = new Random().Next(0, _options.Value.ApiKeys.Length);
            return _options.Value.ApiKeys[random];
        }
    }
}
