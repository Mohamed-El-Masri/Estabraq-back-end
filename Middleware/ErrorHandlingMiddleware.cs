using System.Net;
using System.Text.Json;
using EstabraqTourismAPI.DTOs.Common;

namespace EstabraqTourismAPI.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorHandlingMiddleware(
        RequestDelegate next, 
        ILogger<ErrorHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new ApiResponse<object>
        {
            Success = false,
            Message = "An error occurred while processing your request"
        };

        switch (exception)
        {
            case ArgumentNullException _:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Invalid request parameters";
                response.Errors.Add("One or more required parameters are missing");
                break;

            case ArgumentException _:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Invalid request parameters";
                response.Errors.Add(exception.Message);
                break;

            case UnauthorizedAccessException _:
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Message = "Unauthorized access";
                response.Errors.Add("You are not authorized to perform this action");
                break;

            case KeyNotFoundException _:
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Message = "Resource not found";
                response.Errors.Add("The requested resource was not found");
                break;

            case InvalidOperationException _:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = "Invalid operation";
                response.Errors.Add(exception.Message);
                break;

            case TimeoutException _:
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                response.Message = "Request timeout";
                response.Errors.Add("The request took too long to process");
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Message = "Internal server error";
                
                // Only include detailed error information in development
                if (_environment.IsDevelopment())
                {
                    response.Errors.Add($"Exception: {exception.Message}");
                    response.Errors.Add($"Stack Trace: {exception.StackTrace}");
                }
                else
                {
                    response.Errors.Add("An unexpected error occurred. Please try again later.");
                }
                break;
        }

        // Log the error with correlation ID for tracking
        var correlationId = context.TraceIdentifier;
        _logger.LogError(exception, 
            "Error occurred for request {Method} {Path}. Correlation ID: {CorrelationId}", 
            context.Request.Method, 
            context.Request.Path, 
            correlationId);

        // Add correlation ID to response for debugging
        if (_environment.IsDevelopment())
        {
            response.Errors.Add($"Correlation ID: {correlationId}");
        }

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}
