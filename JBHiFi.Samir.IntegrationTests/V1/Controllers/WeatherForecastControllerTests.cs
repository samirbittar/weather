using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using JBHiFi.Samir.Core.Services;
using JBHiFi.Samir.Web;
using JBHiFi.Samir.Web.V1.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Refit;
using Xunit;

namespace JBHiFi.Samir.IntegrationTests.V1.Controllers
{
    public class WeatherForecastControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public WeatherForecastControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("API-KEY-1")]
        [InlineData("API-KEY-2")]
        [InlineData("API-KEY-3")]
        [InlineData("API-KEY-4")]
        [InlineData("API-KEY-5")]
        public async Task GivenInputDataIsValid_AndApiKeyIsValid_AndOpenWeatherApiReturnsSuccessfulResponse_WhenCallingCurrentWeatherEndpoint_ThenReturnsSuccessResponseWithCurrentWeatherDescription(string apiKey)
        {
            // Arrange
            var client = CreateTestHttpClient(apiKey);

            // Act
            var response = await CallWeatherForecastEndpoint(client, "Melbourne", "AU");

            // Assert
            await AssertSuccessfulResponseAsync(response);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("API-KEY-INVALID")]
        public async Task GivenInputDataIsValid_AndApiKeyIsMissingOrIncorrect_WhenCallingCurrentWeatherEndpoint_ThenReturnsUnauthorisedResponse(string apiKey)
        {
            // Arrange
            var client = CreateTestHttpClient(apiKey);

            // Act
            var response = await CallWeatherForecastEndpoint(client, "Melbourne", "AU");

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task GivenInputDataIsValid_AndApiKeyIsValid_WhenCallingCurrentWeatherEndpointMoreThanFiveTimesInLessThanAnHour_ThenReturnsTooManyRequestsResponseForRateLimitedApiKey_AndReturnsSuccessfulResponseForDifferentApiKey()
        {
            // Arrange
            var clientApiKey1 = CreateTestHttpClient("API-KEY-1");
            var clientApiKey2 = CreateTestHttpClient("API-KEY-2");

            var currentClient = clientApiKey1;

            for (int requestCount = 1; requestCount <= 7; requestCount++)
            {
                // Act
                var response = await CallWeatherForecastEndpoint(currentClient, "Melbourne", "AU");

                // Assert
                if (requestCount <= 5)
                {
                    // Test API-KEY-1 five times. All should succeed.
                    await AssertSuccessfulResponseAsync(response);
                }
                else if (requestCount == 6)
                {
                    // Test API-KEY-1 for the sixth time. Should receive TooManyRequests response. Then,
                    // switch client to API-KEY-2 for seventh request.
                    Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
                    currentClient = clientApiKey2;
                }
                else
                {
                    await AssertSuccessfulResponseAsync(response);
                    // Seventh request uses API-KEY-2. Should succeed.
                }
            }
        }

        private async Task AssertSuccessfulResponseAsync(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

            var responseBody = await response.Content.ReadFromJsonAsync<CurrentWeatherResponse>();
            Assert.NotNull(responseBody);
            Assert.Equal("Test value", responseBody.Description);
        }

        private Task<HttpResponseMessage> CallWeatherForecastEndpoint(HttpClient httpClient, string cityName, string countryCode) =>
            httpClient.GetAsync($"/api/v1/weatherforecast/current?cityName={cityName}&countryCode={countryCode}");

        private HttpClient CreateTestHttpClient(string apiKey = null)
        {
            var testHttpClient = _factory.WithWebHostBuilder(builder =>
            {
                builder
                    .ConfigureAppConfiguration((_, configurationBuilder) =>
                    {
                        // Use same configuration settings as real application.
                        Program.BuildConfigurationSettings(configurationBuilder);
                    })
                    .ConfigureTestServices(services =>
                    {
                        services.AddScoped(_ =>
                        {
                            var testHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                            var testCurrentWeatherApiResponse = new CurrentWeatherApiResponse
                            {
                                Weather = new List<CurrentWeatherApiResponse.WeatherItem>
                                {
                                    new()
                                    {
                                        Description = "Test value"
                                    }
                                }
                            };
                            var testApiResponse = new ApiResponse<CurrentWeatherApiResponse>(testHttpResponseMessage, testCurrentWeatherApiResponse, null!);

                            var testOpenWeatherMapApi = Substitute.For<IOpenWeatherMapApi>();
                            testOpenWeatherMapApi.GetCurrentWeatherAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                                .Returns(testApiResponse);

                            return testOpenWeatherMapApi;
                        });
                    });
            }).CreateClient();
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                testHttpClient.DefaultRequestHeaders.Add("X-ApiKey", new[] { apiKey });
            }

            return testHttpClient;
        }
    }
}
