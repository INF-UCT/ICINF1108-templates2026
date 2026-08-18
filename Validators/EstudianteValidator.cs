using estudiantes_icinf.Models;
using FluentValidation;

namespace estudiantes_icinf.Validators;

public class EstudianteValidator : AbstractValidator<Estudiante>
{
    public EstudianteValidator()
    {
        RuleFor(e => e.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(e => e.Apellido)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido no puede superar los 100 caracteres.");

        RuleFor(e => e.Email)
            .NotEmpty().WithMessage("El email es obligatorio.")
            .EmailAddress().WithMessage("El email no tiene un formato valido.");

        RuleFor(e => e.Carrera)
            .NotEmpty().WithMessage("La carrera es obligatoria.")
            .MaximumLength(100).WithMessage("La carrera no puede superar los 100 caracteres.");
    }
}
