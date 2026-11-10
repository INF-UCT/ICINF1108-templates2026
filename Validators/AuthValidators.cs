using estudiantes_icinf.Models;
using FluentValidation;

namespace estudiantes_icinf.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        RuleFor(r => r.Name).NotEmpty().WithMessage("El nombre no puede estar vacío").StudentName();

        RuleFor(r => r.Email)
            .NotEmpty().WithMessage("El correo electrónico no puede estar vacío")
            .ValidEmail();

        RuleFor(r => r.Password)
            .NotEmpty().WithMessage("La contraseña no puede estar vacía")
            .Length(8, 72).WithMessage("La contraseña debe tener entre 8 y 72 caracteres");
    }
}

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(l => l.Email)
            .NotEmpty().WithMessage("El correo electrónico no puede estar vacío")
            .ValidEmail();

        RuleFor(l => l.Password).NotEmpty().WithMessage("La contraseña no puede estar vacía");
    }
}
