using System.Net;
using System.Text.Json;
using AuthService.Application.Exceptions;

namespace AuthService.API.Middlewares;

/// <summary>
/// Global exception handling middleware.
/// </summary>
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UserAlreadyExistsException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.Conflict);
        }
        catch (InvalidCredentialsException ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.Unauthorized);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex, HttpStatusCode.InternalServerError);
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context, 
        Exception exception, 
        HttpStatusCode statusCode)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            error = exception.Message,
            statusCode = (int)statusCode
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
