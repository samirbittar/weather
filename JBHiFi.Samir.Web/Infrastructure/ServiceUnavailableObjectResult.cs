using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace JBHiFi.Samir.Web.Infrastructure
{
    /// <summary>
    /// An <see cref="ObjectResult"/> that when executed will produce a Service Unavailable (503) response.
    /// </summary>
    public class ServiceUnavailableObjectResult : ObjectResult
    {
        /// <summary>
        /// Creates a new <see cref="ServiceUnavailableObjectResult"/> instance.
        /// </summary>
        /// <param name="value">The value to format in the entity body.</param>
        public ServiceUnavailableObjectResult([ActionResultObjectValue] object value) : base(value) =>
            StatusCode = StatusCodes.Status503ServiceUnavailable;
    }
}
