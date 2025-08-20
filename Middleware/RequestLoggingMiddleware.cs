using System.Diagnostics;

namespace EstabraqTourismAPI.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = context.TraceIdentifier;

        // Log request start
        _logger.LogInformation(
            "Request started: {Method} {Path} {QueryString} - Correlation ID: {CorrelationId}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString,
            correlationId);

        // Add correlation ID to response headers
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            // Log request completion
            _logger.LogInformation(
                "Request completed: {Method} {Path} {StatusCode} in {ElapsedMilliseconds}ms - Correlation ID: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                correlationId);

            // Log slow requests
            if (stopwatch.ElapsedMilliseconds > 5000) // 5 seconds
            {
                _logger.LogWarning(
                    "Slow request detected: {Method} {Path} took {ElapsedMilliseconds}ms - Correlation ID: {CorrelationId}",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds,
                    correlationId);
            }
        }
    }
}
