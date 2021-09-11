using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JBHiFi.Samir.Core.Queries;
using JBHiFi.Samir.Web.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace JBHiFi.Samir.Web.V1.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WeatherController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get the current weather for a specific city in a specific country.
        /// </summary>
        /// <param name="cityName">The city name.</param>
        /// <param name="countryCode">The ISO-3166 country code.</param>
        /// <returns></returns>
        [HttpGet("current")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CurrentWeatherResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Current(string cityName, string countryCode, CancellationToken cancellationToken)
        {
            if (!ValidateParameters(out var errors))
            {
                return ValidationProblem(new ValidationProblemDetails(errors));
            }

            var query = new CurrentWeather.Query(cityName, countryCode);

            var queryResult = await _mediator.Send(query, cancellationToken);
            if (queryResult.IsSuccess)
            {
                var currentWeatherResult = new CurrentWeatherResponse { Description = queryResult.Description };
                return Ok(currentWeatherResult);
            }

            return queryResult.Status == CurrentWeather.QueryResult.StatusResultValues.NotFound
                ? NotFound(new ValidationProblemDetails { Title = "Could not find city and/or country.", Status = (int)HttpStatusCode.NotFound })
                : StatusCode((int)HttpStatusCode.InternalServerError);

            // Local functions
            bool ValidateParameters(out Dictionary<string, string[]> errors)
            {
                errors = new Dictionary<string, string[]>();

                if (string.IsNullOrWhiteSpace(cityName))
                {
                    errors.Add(nameof(cityName), new[] { $"Parameter {nameof(cityName)} is required." });
                }

                if (string.IsNullOrWhiteSpace(countryCode))
                {
                    errors.Add(nameof(countryCode), new[] { $"Parameter {nameof(countryCode)} is required." });
                }

                return errors.Count == 0;
            }
        }
    }
}
