using System.Net;
using System.Text.Json;
using TradeHub.Api.Utilities;

namespace TradeHub.Api.Middleware;

public class GlobalExceptionHandlerMiddleware(
    RequestDelegate next,
    ILogger<GlobalExceptionHandlerMiddleware> logger
)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

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

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(
            exception,
            "An unhandled exception has occurred: {Message}",
            exception.Message
        );

        ApiExceptionResponse errorResponse = exception switch
        {
            ValidationException ex => ex,
            NotFoundException ex => ex,
            ConflictException ex => ex,
            BadRequestException ex => ex,

            ApiException ex => ex,
            _ => new ApiExceptionResponse
            {
                Title = "Internal Server Error",
                Status = (int)HttpStatusCode.InternalServerError,
                Detail = "An unexpected error occurred. Please try again later.",
            },
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorResponse.Status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}
