using System.Net;
using System.Security.Authentication;
using System.Text.Json;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TicketAnnd.Application;
using TicketAnnd.Extensions;

namespace TicketAnnd;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var message = exception.Message;

        if (exception is AppException appEx)
        {
            statusCode = (HttpStatusCode)appEx.StatusCode;
            message = appEx.Message;
        }
        else if (exception is ValidationException validationEx)
        {
            statusCode = HttpStatusCode.BadRequest;
            var errors = validationEx.Errors
                .Select(e => new ValidationErrorDto { PropertyName = e.PropertyName, ErrorMessage = e.ErrorMessage })
                .ToList();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;
            return context.Response.WriteAsync(JsonSerializer.Serialize(new ValidationErrorResponse
            {
                StatusCode = (int)statusCode,
                Message = "Validation failed.",
                Errors = errors
            }));
        }
        else
        {
            switch (exception)
            {
                case SecurityTokenException:
                case InvalidCredentialException:
                    statusCode = HttpStatusCode.Unauthorized;
                    message = "Unauthorized";
                    break;

                case ArgumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;

                case DbUpdateException dbEx when dbEx.IsUniqueConstraintViolation():
                    statusCode = HttpStatusCode.Conflict;
                    message = "A resource with this value already exists.";
                    break;

                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = exception.Message;
                    break;
            }
        }


        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ValidationErrorResponse : ErrorResponse
{
    public List<ValidationErrorDto> Errors { get; set; } = new();
}

public class ValidationErrorDto
{
    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}