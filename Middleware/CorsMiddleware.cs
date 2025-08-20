namespace EstabraqTourismAPI.Middleware;

public class CorsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorsMiddleware> _logger;

    public CorsMiddleware(RequestDelegate next, ILogger<CorsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // Add CORS headers
        AddCorsHeaders(context);

        // Handle preflight requests
        if (context.Request.Method == "OPTIONS")
        {
            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(string.Empty);
            return;
        }

        await _next(context);
    }

    private void AddCorsHeaders(HttpContext context)
    {
        var headers = context.Response.Headers;
        var origin = context.Request.Headers["Origin"].ToString();

        // Allow specific origins or all origins for development
        if (!string.IsNullOrEmpty(origin))
        {
            // In production, you should specify exact origins
            var allowedOrigins = new[]
            {
                "http://localhost:3000",      // React development
                "http://localhost:5173",      // Vite development
                "http://localhost:4200",      // Angular development
                "https://estabraqtourism.com", // Production domain
                "https://www.estabraqtourism.com"
            };

            if (allowedOrigins.Contains(origin) || 
                origin.StartsWith("http://localhost:") || 
                origin.StartsWith("https://localhost:"))
            {
                headers["Access-Control-Allow-Origin"] = origin;
            }
        }

        headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS, PATCH";
        headers["Access-Control-Allow-Headers"] = 
            "Content-Type, Authorization, X-Requested-With, Accept, Origin, Cache-Control, X-File-Name";
        headers["Access-Control-Allow-Credentials"] = "true";
        headers["Access-Control-Max-Age"] = "86400"; // 24 hours

        // Expose custom headers to the client
        headers["Access-Control-Expose-Headers"] = "X-Correlation-ID, X-RateLimit-Limit, X-RateLimit-Remaining, X-RateLimit-Reset";
    }
}
