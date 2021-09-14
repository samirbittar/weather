using Microsoft.AspNetCore.Builder;

namespace Weather.Web.Infrastructure.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyVerification(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VerifyApiKeyMiddleware>();
        }
    }
}