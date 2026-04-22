using Microsoft.AspNetCore.Http;
using System.Net;
using FluentValidation;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Common.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new { isSuccess = false, data = (object?)null, message = "", errors = new List<string>() };

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var errorMessages = validationException.Errors
                    .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                    .ToList();
                
                response = new
                {
                    isSuccess = false,
                    data = (object?)null,
                    message = "Validation failed",
                    errors = errorMessages
                };
                break;

            case DbUpdateException dbException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var innerMessage = dbException.InnerException?.Message ?? dbException.Message;
                response = new
                {
                    isSuccess = false,
                    data = (object?)null,
                    message = "Database error occurred",
                    errors = new List<string> { innerMessage }
                };
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                var messageWithInner = exception.InnerException != null 
                    ? $"{exception.Message} -> {exception.InnerException.Message}"
                    : exception.Message;
                    
                response = new
                {
                    isSuccess = false,
                    data = (object?)null,
                    message = "An unexpected error occurred",
                    errors = new List<string> { messageWithInner }
                };
                break;
        }

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(response, options);
        return context.Response.WriteAsync(json);
    }
}
