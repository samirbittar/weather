using Microsoft.AspNetCore.Builder;

namespace JBHiFi.Samir.Web.Infrastructure.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseApiKeyVerification(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VerifyApiKeyMiddleware>();
        }
    }
}