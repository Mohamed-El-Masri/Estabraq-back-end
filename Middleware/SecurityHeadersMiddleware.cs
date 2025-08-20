using EstabraqTourismAPI.Configuration;

namespace EstabraqTourismAPI.Middleware;

public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly SecuritySettings _securitySettings;
    private readonly ILogger<SecurityHeadersMiddleware> _logger;

    public SecurityHeadersMiddleware(
        RequestDelegate next, 
        SecuritySettings securitySettings,
        ILogger<SecurityHeadersMiddleware> logger)
    {
        _next = next;
        _securitySettings = securitySettings;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // Add security headers
        AddSecurityHeaders(context);

        await _next(context);
    }

    private void AddSecurityHeaders(HttpContext context)
    {
        var headers = context.Response.Headers;

        // Remove server information
        headers.Remove("Server");

        // X-Content-Type-Options
        if (!headers.ContainsKey("X-Content-Type-Options"))
        {
            headers["X-Content-Type-Options"] = "nosniff";
        }

        // X-Frame-Options
        if (!headers.ContainsKey("X-Frame-Options"))
        {
            headers["X-Frame-Options"] = "DENY";
        }

        // X-XSS-Protection
        if (!headers.ContainsKey("X-XSS-Protection"))
        {
            headers["X-XSS-Protection"] = "1; mode=block";
        }

        // Referrer-Policy
        if (!headers.ContainsKey("Referrer-Policy"))
        {
            headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        }

        // Content-Security-Policy
        if (_securitySettings.EnableCSP && !headers.ContainsKey("Content-Security-Policy"))
        {
            var cspPolicy = BuildContentSecurityPolicy();
            headers["Content-Security-Policy"] = cspPolicy;
        }

        // Strict-Transport-Security (HSTS)
        if (_securitySettings.EnableHSTS && context.Request.IsHttps && !headers.ContainsKey("Strict-Transport-Security"))
        {
            headers["Strict-Transport-Security"] = $"max-age={_securitySettings.HSTSMaxAge}; includeSubDomains";
        }

        // Permissions-Policy
        if (!headers.ContainsKey("Permissions-Policy"))
        {
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
        }
    }

    private string BuildContentSecurityPolicy()
    {
        var policies = new List<string>
        {
            "default-src 'self'",
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'",
            "style-src 'self' 'unsafe-inline'",
            "img-src 'self' data: https:",
            "font-src 'self' data:",
            "connect-src 'self'",
            "media-src 'self'",
            "object-src 'none'",
            "frame-ancestors 'none'",
            "base-uri 'self'",
            "form-action 'self'"
        };

        return string.Join("; ", policies);
    }
}
