using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JBHiFi.Samir.Core.Queries;
using JBHiFi.Samir.Web.Infrastructure;
using JBHiFi.Samir.Web.V1.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JBHiFi.Samir.Web.V1.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/weatherforecast")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WeatherForecastController(IMediator mediator)
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
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(typeof(CurrentWeatherResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> Current(string cityName, string countryCode, CancellationToken cancellationToken)
        {
            if (!ValidateParameters(out var validationErrors))
            {
                return ValidationProblem(new ValidationProblemDetails(validationErrors));
            }

            var query = new CurrentWeather.Query(cityName, countryCode);

            var queryResult = await _mediator.Send(query, cancellationToken);
            if (queryResult.IsSuccess)
            {
                var currentWeatherResult = new CurrentWeatherResponse { Description = queryResult.Description };
                return Ok(currentWeatherResult);
            }

            return queryResult.Status switch
            {
                CurrentWeather.QueryResult.StatusResultValues.NotFound => NotFound("Could not find city and/or country."),
                CurrentWeather.QueryResult.StatusResultValues.ServiceUnavailable => ServiceUnavailable(),
                _ => InternalServerError()
            };

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

        private ServiceUnavailableObjectResult ServiceUnavailable() =>
            new(new ProblemDetails { Title = "Your request could not be serviced at this time.", Status = StatusCodes.Status503ServiceUnavailable });

        private InternalServerErrorObjectResult InternalServerError() =>
            new(new ProblemDetails { Title = "An internal error occured when processing your request.", Status = StatusCodes.Status500InternalServerError});

        private NotFoundObjectResult NotFound(string detail) =>
            new(new ProblemDetails { Title = "Not found", Detail = detail, Status = StatusCodes.Status404NotFound });
    }
}
