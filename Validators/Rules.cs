using FluentValidation;

namespace estudiantes_icinf.Validators;

// Reglas compartidas por varios validadores, con los mismos mensajes que
// usa la versión NestJS de la API.
public static class Rules
{
    private const string NoHtmlPattern = "^[^<>]*$";
    private const string NoHtmlMessage = "No se permiten etiquetas HTML";

    public static IRuleBuilderOptions<T, string?> NoHtml<T>(this IRuleBuilder<T, string?> rule) =>
        rule.Matches(NoHtmlPattern).WithMessage(NoHtmlMessage);

    public static IRuleBuilderOptions<T, string?> StudentName<T>(this IRuleBuilder<T, string?> rule) =>
        rule.Length(3, 100).WithMessage("El nombre debe tener entre 3 y 100 caracteres").NoHtml();

    public static IRuleBuilderOptions<T, string?> Username<T>(this IRuleBuilder<T, string?> rule) =>
        rule.Length(3, 30).WithMessage("El nombre de usuario debe tener entre 3 y 30 caracteres")
            .Matches("^[a-z0-9_]+$")
            .WithMessage("El nombre de usuario solo puede contener minúsculas, números y guiones bajos");

    public static IRuleBuilderOptions<T, string?> ValidEmail<T>(this IRuleBuilder<T, string?> rule) =>
        rule.EmailAddress().WithMessage("El correo electrónico no es válido");

    public static IRuleBuilderOptions<T, int?> StudentAge<T>(this IRuleBuilder<T, int?> rule) =>
        rule.GreaterThanOrEqualTo(18).WithMessage("La edad debe ser al menos 18")
            .LessThanOrEqualTo(99).WithMessage("La edad debe ser como máximo 99");

    public static IRuleBuilderOptions<T, string?> PetText<T>(this IRuleBuilder<T, string?> rule, string field) =>
        rule.Length(1, 50).WithMessage($"{field} debe tener entre 1 y 50 caracteres").NoHtml();

    public static IRuleBuilderOptions<T, int?> PetAge<T>(this IRuleBuilder<T, int?> rule) =>
        rule.GreaterThanOrEqualTo(0).WithMessage("La edad debe ser al menos 0")
            .LessThanOrEqualTo(100).WithMessage("La edad debe ser como máximo 100");
}
