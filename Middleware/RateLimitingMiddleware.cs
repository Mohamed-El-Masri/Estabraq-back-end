using System.Net;
using System.Text.Json;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly Dictionary<string, ClientRequestInfo> _clients;
    private readonly int _maxRequestsPerMinute;
    private readonly TimeSpan _timeWindow;

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        _clients = new Dictionary<string, ClientRequestInfo>();
        _maxRequestsPerMinute = 100; // Default rate limit
        _timeWindow = TimeSpan.FromMinutes(1);
    }

    public async Task Invoke(HttpContext context)
    {
        var clientId = GetClientIdentifier(context);
        
        if (IsRateLimited(clientId))
        {
            await HandleRateLimitExceeded(context, clientId);
            return;
        }

        TrackRequest(clientId);
        await _next(context);
    }

    private string GetClientIdentifier(HttpContext context)
    {
        // Try to get user ID if authenticated
        var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user_{userId}";
        }

        // Fall back to IP address
        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
        return $"ip_{ipAddress ?? "unknown"}";
    }

    private bool IsRateLimited(string clientId)
    {
        lock (_clients)
        {
            if (!_clients.ContainsKey(clientId))
            {
                return false;
            }

            var clientInfo = _clients[clientId];
            var now = DateTime.UtcNow;

            // Remove old requests outside the time window
            clientInfo.Requests.RemoveAll(r => now - r > _timeWindow);

            return clientInfo.Requests.Count >= _maxRequestsPerMinute;
        }
    }

    private void TrackRequest(string clientId)
    {
        lock (_clients)
        {
            if (!_clients.ContainsKey(clientId))
            {
                _clients[clientId] = new ClientRequestInfo();
            }

            var clientInfo = _clients[clientId];
            var now = DateTime.UtcNow;

            // Remove old requests outside the time window
            clientInfo.Requests.RemoveAll(r => now - r > _timeWindow);
            
            // Add current request
            clientInfo.Requests.Add(now);

            // Clean up clients that haven't made requests recently
            if (_clients.Count > 1000) // Prevent memory issues
            {
                var cutoffTime = now.Subtract(TimeSpan.FromHours(1));
                var clientsToRemove = _clients
                    .Where(kv => kv.Value.Requests.Count == 0 || kv.Value.Requests.Max() < cutoffTime)
                    .Select(kv => kv.Key)
                    .ToList();

                foreach (var client in clientsToRemove)
                {
                    _clients.Remove(client);
                }
            }
        }
    }

    private async Task HandleRateLimitExceeded(HttpContext context, string clientId)
    {
        context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        context.Response.ContentType = "application/json";

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "Rate limit exceeded",
            Errors = new List<string> { "Too many requests. Please try again later." }
        };

        // Add rate limit headers
        context.Response.Headers["X-RateLimit-Limit"] = _maxRequestsPerMinute.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = "0";
        context.Response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.Add(_timeWindow).ToUnixTimeSeconds().ToString();

        _logger.LogWarning("Rate limit exceeded for client {ClientId}", clientId);

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private class ClientRequestInfo
    {
        public List<DateTime> Requests { get; set; } = new List<DateTime>();
    }
}
