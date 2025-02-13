using System.Text.Json;
using System.Text.Json.Serialization;
using KServerTools.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace server;

public record ErrorResponse(
    [property:JsonPropertyName("message")] string Message, 
    [property:JsonPropertyName("errorCode")] string ErrorCode) {
    static public ErrorResponse FromServiceException(ServiceException exception) => new(
            exception.Message,
            exception.ServiceError.ToString());
}

internal class ExceptionHandler(IJsonLogger logger) : IExceptionHandler {
    private readonly IJsonLogger logger = logger;
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
        await ErrorHandler.HandleExceptionAsync(
            this.logger,
            httpContext,
            exception, 
            cancellationToken).ConfigureAwait(false);
        return true;
    }
}

internal static class ErrorHandler {
    public static async Task HandleExceptionAsync(IJsonLogger logger, HttpContext httpContext, Exception? exception, CancellationToken cancellationToken) {
        if (exception != null && exception is ServiceException serviceException) {
            ErrorResponse response = ErrorResponse.FromServiceException(serviceException);
            httpContext.Response.StatusCode = (int)serviceException.ServiceError;
            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsync(JsonSerializer.Serialize(response), cancellationToken)
                .ConfigureAwait(false);
        } else {
            logger.IfError(exception != null, "Unhandled exception", exception!);
            httpContext.Response.StatusCode = 500;
            httpContext.Response.ContentType = "text/plain";
            await httpContext.Response.WriteAsync("Server Unavailable", cancellationToken)
                .ConfigureAwait(false);
        }
    }
}