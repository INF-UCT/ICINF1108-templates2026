using estudiantes_icinf.Models;
using FluentValidation;

namespace estudiantes_icinf.Validators;

public class ProfesorValidator : AbstractValidator<Profesor>
{
    public ProfesorValidator()
    {
        RuleFor(e => e.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(e => e.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato valido.");

        RuleFor(e => e.FechaIngreso)
            .NotEmpty().WithMessage("El Año es obligatorio.")
            .LessThan(1999).WithMessage("Año no valido");
        }
}
