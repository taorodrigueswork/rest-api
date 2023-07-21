using System.Net;
using System.Text.Json;

namespace ExceptionHandling.CustomMiddleware;

/// <summary>
/// For more information: https://code-maze.com/global-error-handling-aspnetcore/
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;

        switch (exception)
        {
            case ApplicationException ex:
                if (ex.Message.Contains("Invalid Token"))
                {
                    response.StatusCode = (int)HttpStatusCode.Forbidden;
                    break;
                }
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            case ArgumentNullException ex:
                if (ex.Message.Contains("not found"))
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    break;
                }
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        var logMessage = $"""{exception}, "ErrorHandlingMiddleware" | [Method]: "HandleExceptionAsync" | Error: {exception.Message} Headers: {context.Request.Headers}. Path: {context.Request.Path}. Body: {context.Request.Body}""";

        _logger.LogError(logMessage);

        var result = JsonSerializer.Serialize(exception.Message);
        await context.Response.WriteAsync(result);
    }
}