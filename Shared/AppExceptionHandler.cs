using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace estudiantes_icinf.Shared;

// Convierte cualquier excepción en una respuesta con el envelope estándar.
public class AppExceptionHandler(ILogger<AppExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var response = exception switch
        {
            ApiException api => ApiResponse.Fail(api.StatusCode, api.Message, api.Errors),

            // JSON mal formado o con tipos incorrectos (por ejemplo, "age": "veinte").
            BadHttpRequestException => ApiResponse.Fail(
                StatusCodes.Status400BadRequest,
                "El cuerpo de la petición no es un JSON válido"),

            // Respaldo por si dos peticiones simultáneas pasan la validación de
            // unicidad del servicio: la restricción UNIQUE de Postgres responde.
            DbUpdateException { InnerException: PostgresException { SqlState: PostgresErrorCodes.UniqueViolation } } =>
                ApiResponse.Fail(StatusCodes.Status409Conflict, "Conflicto con datos existentes"),

            _ => null,
        };

        if (response is null)
        {
            logger.LogError(exception, "Error no controlado");
            response = ApiResponse.Fail(StatusCodes.Status500InternalServerError, "Error interno del servidor");
        }

        httpContext.Response.StatusCode = response.Code;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}
