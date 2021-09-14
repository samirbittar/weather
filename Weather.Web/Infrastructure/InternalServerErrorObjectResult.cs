using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Weather.Web.Infrastructure
{
    /// <summary>
    /// An <see cref="ObjectResult"/> that when executed will produce a Internal Server Error (500) response.
    /// </summary>
    public class InternalServerErrorObjectResult : ObjectResult
    {
        /// <summary>
        /// Creates a new <see cref="InternalServerErrorObjectResult"/> instance.
        /// </summary>
        /// <param name="value">The value to format in the entity body.</param>
        public InternalServerErrorObjectResult([ActionResultObjectValue] object value) : base(value) =>
            StatusCode = StatusCodes.Status500InternalServerError;
    }
}