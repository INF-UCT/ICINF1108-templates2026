using System.Text.Json;
using FluentValidation;

namespace estudiantes_icinf.Shared;

// Valida el cuerpo de la petición con el IValidator<T> registrado. Si falla,
// responde 400 con los errores agrupados por campo: { campo: { message } }.
public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        var body = context.Arguments.OfType<T>().FirstOrDefault();

        if (validator is null || body is null)
        {
            return await next(context);
        }

        var result = await validator.ValidateAsync(body);

        if (result.IsValid)
        {
            return await next(context);
        }

        // Si un campo falla varias reglas, los mensajes se unen con ", ".
        var errors = result.Errors
            .GroupBy(e => ToCamelCase(e.PropertyName))
            .ToDictionary(
                g => g.Key,
                g => new FieldError(string.Join(", ", g.Select(e => e.ErrorMessage).Distinct())));

        return ApiResults.Fail(StatusCodes.Status400BadRequest, "Error de validación", errors);
    }

    // "Pets[0].Name" -> "pets[0].name", para que coincida con el JSON.
    private static string ToCamelCase(string propertyPath) =>
        string.Join('.', propertyPath.Split('.').Select(JsonNamingPolicy.CamelCase.ConvertName));
}
