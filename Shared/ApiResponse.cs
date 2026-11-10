using System.Text.Json.Serialization;

namespace estudiantes_icinf.Shared;

public record FieldError(string Message);

// Envelope común para todas las respuestas de la API, exitosas o de error.
public class ApiResponse
{
    public int Code { get; init; }
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string Timestamp { get; init; } = Now();
    public object? Data { get; init; }

    // Error simple (404, 401, 500). Se omite del JSON cuando no aplica.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Error { get; init; }

    // Errores por campo (400, 409). Se omite del JSON cuando no aplica.
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Dictionary<string, FieldError>? Errors { get; init; }

    public static ApiResponse Ok(int code, string message, object? data) =>
        new() { Code = code, Success = true, Message = message, Data = data };

    public static ApiResponse Fail(int code, string message, Dictionary<string, FieldError>? errors = null) =>
        new()
        {
            Code = code,
            Success = false,
            Message = message,
            Error = errors is null ? message : null,
            Errors = errors,
        };

    private static string Now() => DateTime.UtcNow.ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'");
}

// Atajos para devolver el envelope con el código y mensaje de cada método HTTP.
public static class ApiResults
{
    public static IResult Ok(object? data) => Json(ApiResponse.Ok(200, "Operación exitosa", data));

    public static IResult Created(object? data) => Json(ApiResponse.Ok(201, "Recurso creado", data));

    public static IResult Updated(object? data) => Json(ApiResponse.Ok(200, "Recurso actualizado", data));

    public static IResult Deleted(object? data) => Json(ApiResponse.Ok(200, "Recurso eliminado", data));

    public static IResult Fail(int code, string message, Dictionary<string, FieldError>? errors = null) =>
        Json(ApiResponse.Fail(code, message, errors));

    private static IResult Json(ApiResponse response) =>
        Results.Json(response, statusCode: response.Code);
}
