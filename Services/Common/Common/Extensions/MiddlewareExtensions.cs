using Common.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Common.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }

    public static IApplicationBuilder UseRequestCorrelation(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestCorrelationMiddleware>();
    }
}
