using Microsoft.AspNetCore.Mvc;

namespace JBHiFi.Samir.V1.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WeatherController : ControllerBase
    {
        /// <summary>
        /// Get the current weather for a specific city in a specific country.
        /// </summary>
        /// <param name="cityName">The city name.</param>
        /// <param name="countryCode">The ISO-3166 country code.</param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult Current(string cityName, string countryCode)
        {
            return Ok("Hello, World!");
        }
    }
}
