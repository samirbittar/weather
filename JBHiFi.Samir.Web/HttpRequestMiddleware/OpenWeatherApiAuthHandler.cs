using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JBHiFi.Samir.Core.Services;

namespace JBHiFi.Samir.Web.HttpRequestMiddleware
{
    public class OpenWeatherApiAuthHandler : DelegatingHandler
    {
        private readonly IOpenWeatherMapApiKeyProvider _apiKeyProvider;

        public OpenWeatherApiAuthHandler(IOpenWeatherMapApiKeyProvider apiKeyProvider)
        {
            _apiKeyProvider = apiKeyProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var apiKey = _apiKeyProvider.GetApiKey();
            request.RequestUri = new Uri($"{request.RequestUri}&appid={apiKey}");

            return base.SendAsync(request, cancellationToken);
        }
    }
}
