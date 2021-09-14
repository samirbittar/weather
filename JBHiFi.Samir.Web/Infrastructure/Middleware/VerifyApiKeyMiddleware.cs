using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JBHiFi.Samir.Web.Infrastructure.Middleware
{
    public class VerifyApiKeyMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly List<string> _knownApiKeys = new() { "API-KEY-1", "API-KEY-2", "API-KEY-3", "API-KEY-4", "API-KEY-5" };

        public VerifyApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Ensure we only check for API keys when accessing the API.
            if (IsApiCall())
            {
                if (IsApiKeyPresent(out var apiKey) && IsKnownApiKey(apiKey))
                {
                    await _next(context);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
            }
            else
            {
                await _next(context);
            }


            // Local functions
            bool IsApiCall()
            {
                return context.Request.Path.HasValue && context.Request.Path.Value!.StartsWith("/api/");
            }

            bool IsApiKeyPresent(out string apiKey)
            {
                if (context.Request.Headers.TryGetValue("X-ApiKey", out var apiKeyValues) && apiKeyValues.Count == 1)
                {
                    apiKey = apiKeyValues[0];
                    return true;
                }

                apiKey = null;
                return false;
            }

            bool IsKnownApiKey(string apiKey)
            {
                // Comparison is case-sensitive on purpose.
                return _knownApiKeys.Contains(apiKey);
            }
        }
    }
}
