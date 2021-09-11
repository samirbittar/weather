using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JBHiFi.Samir.Core.Services;
using MediatR;

namespace JBHiFi.Samir.Core.Queries
{
    public class CurrentWeather
    {
        public class Query : IRequest<QueryResult>
        {
            public Query(string cityName, string countryCode)
            {
                CityName = cityName;
                CountryCode = countryCode;
            }

            public string CityName { get; }
            public string CountryCode { get; }
        }

        public class QueryResult
        {
            public static QueryResult Success(string description) => new(StatusResultValues.Success, description: description);
            public static QueryResult NotFound() => new(StatusResultValues.NotFound);
            public static QueryResult Error(string errorMessage) => new(StatusResultValues.Error, errorMessage: errorMessage);

            private QueryResult(StatusResultValues status, string description = null, string errorMessage = null)
            {
                Status = status;
                Description = description;
                ErrorMessage = errorMessage;
            }

            public StatusResultValues Status { get; }
            public bool IsSuccess => Status == StatusResultValues.Success;
            public string Description { get; }
            public string ErrorMessage { get; }

            public enum StatusResultValues
            {
                Success,
                NotFound,
                Error
            }
        }

        public class QueryHandler : IRequestHandler<Query, QueryResult>
        {
            private readonly IOpenWeatherMapApi _weatherApi;

            public QueryHandler(IOpenWeatherMapApi weatherApi)
            {
                _weatherApi = weatherApi;
            }

            public async Task<QueryResult> Handle(Query request, CancellationToken cancellationToken)
            {
                var apiResponse = await _weatherApi.GetCurrentWeather(request.CityName, request.CountryCode);
                if (apiResponse.IsSuccessStatusCode)
                {
                    var weatherDescription = apiResponse.Content?.Weather.FirstOrDefault()?.Description;
                    return QueryResult.Success(weatherDescription);
                }

                return apiResponse.StatusCode == HttpStatusCode.NotFound
                    ? QueryResult.NotFound()
                    : QueryResult.Error(apiResponse.Error?.Message);
            }
        }
    }
}
