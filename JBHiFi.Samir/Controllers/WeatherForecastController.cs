using Microsoft.AspNetCore.Mvc;

namespace JBHiFi.Samir.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
    }
}
