using System.Net;
using Microsoft.Data.SqlClient;

namespace FestivalCine.Common;

public sealed class SqlExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SqlExceptionHandlingMiddleware> _logger;

    public SqlExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<SqlExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (SqlException ex)
        {
            _logger.LogWarning(ex, "SQL Server rechazo la operacion solicitada.");

            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Fail(GetCleanSqlMessage(ex));
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Intento de acceso no autorizado.");

            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Fail(ex.Message);
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inesperado en la API.");

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var response = ApiResponse<object>.Fail("Ocurrio un error inesperado. Intenta nuevamente.");
            await context.Response.WriteAsJsonAsync(response);
        }
    }

    private static string GetCleanSqlMessage(SqlException exception)
    {
        var message = exception.Errors.Count > 0
            ? exception.Errors[0].Message
            : exception.Message;

        return string.IsNullOrWhiteSpace(message)
            ? "SQL Server no pudo completar la operacion."
            : message;
    }
}
