namespace estudiantes_icinf.Shared;

// Error de negocio que AppExceptionHandler convierte en una respuesta con envelope.
public class ApiException(int statusCode, string message, Dictionary<string, FieldError>? errors = null)
    : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public Dictionary<string, FieldError>? Errors { get; } = errors;

    public static ApiException NotFound(string message) => new(StatusCodes.Status404NotFound, message);

    public static ApiException Unauthorized(string message) =>
        new(StatusCodes.Status401Unauthorized, message);

    public static ApiException Conflict(Dictionary<string, FieldError> errors) =>
        new(StatusCodes.Status409Conflict, "Conflicto con datos existentes", errors);

    public static ApiException Conflict(string field, string message) =>
        Conflict(new Dictionary<string, FieldError> { [field] = new FieldError(message) });
}
