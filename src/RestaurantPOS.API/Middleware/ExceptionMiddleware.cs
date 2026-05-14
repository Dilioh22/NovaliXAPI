using Microsoft.AspNetCore.Mvc;
using RestaurantPOS.Application.Common.Exceptions;
using System.Net;
using System.Text.Json;

namespace RestaurantPOS.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";

        var problemDetails = exception switch
        {
            NotFoundException notFound => new ProblemDetails
            {
                Status = (int)HttpStatusCode.NotFound,
                Title = "Not Found",
                Detail = notFound.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4"
            },
            BusinessRuleException businessRule => new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Business Rule Violation",
                Detail = businessRule.Message,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            },
            UnauthorizedAccessException => new ProblemDetails
            {
                Status = (int)HttpStatusCode.Unauthorized,
                Title = "Unauthorized",
                Detail = "No autorizado.",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1"
            },
            _ => new ProblemDetails
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Title = "Server Error",
                Detail = _env.IsDevelopment() ? exception.ToString() : "Ocurrió un error interno del servidor.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
            }
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;
        context.Response.StatusCode = problemDetails.Status!.Value;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
    }
}
