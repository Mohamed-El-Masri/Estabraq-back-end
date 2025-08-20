namespace EstabraqTourismAPI.Middleware;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds custom JWT middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseCustomJwt(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JwtMiddleware>();
    }

    /// <summary>
    /// Adds global error handling middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }

    /// <summary>
    /// Adds request logging middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }

    /// <summary>
    /// Adds rate limiting middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RateLimitingMiddleware>();
    }

    /// <summary>
    /// Adds security headers middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }

    /// <summary>
    /// Adds custom CORS middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorsMiddleware>();
    }

    /// <summary>
    /// Adds all custom middlewares in the correct order
    /// </summary>
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder builder)
    {
        return builder
            .UseErrorHandling()         // Should be first to catch all errors
            .UseRequestLogging()        // Log all requests
            .UseSecurityHeaders()       // Add security headers
            .UseCustomCors()           // Handle CORS
            .UseRateLimiting()         // Rate limiting
            .UseCustomJwt();           // JWT processing
    }
}
