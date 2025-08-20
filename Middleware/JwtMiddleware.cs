using System.Security.Claims;

namespace EstabraqTourismAPI.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JwtMiddleware> _logger;

    public JwtMiddleware(RequestDelegate next, ILogger<JwtMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if (token != null)
        {
            await AttachUserToContext(context, token);
        }

        await _next(context);
    }

    private Task AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            // The JWT authentication is handled by the built-in JWT middleware
            // This middleware can be used for additional token processing if needed
            
            // For now, we'll just log the token usage
            var userIdClaim = context.User?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                _logger.LogDebug("User {UserId} authenticated via JWT", userIdClaim.Value);
            }
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error processing JWT token");
            return Task.CompletedTask;
        }
    }
}
