using estudiantes_icinf.Models;
using FluentValidation;

namespace estudiantes_icinf.Validators;

public class CreateStudentValidator : AbstractValidator<CreateStudentDto>
{
    public CreateStudentValidator()
    {
        RuleFor(s => s.Name).NotEmpty().WithMessage("El nombre no puede estar vacío").StudentName();

        RuleFor(s => s.Username)
            .NotEmpty().WithMessage("El nombre de usuario no puede estar vacío")
            .Username();

        RuleFor(s => s.Email)
            .NotEmpty().WithMessage("El correo electrónico no puede estar vacío")
            .ValidEmail();

        RuleFor(s => s.Age).NotNull().WithMessage("La edad es obligatoria").StudentAge();

        RuleForEach(s => s.Pets).SetValidator(new CreatePetValidator());
    }
}

// En la actualización todos los campos son opcionales: solo se validan si vienen.
public class UpdateStudentValidator : AbstractValidator<UpdateStudentDto>
{
    public UpdateStudentValidator()
    {
        RuleFor(s => s.Name).StudentName().When(s => s.Name is not null);
        RuleFor(s => s.Username).Username().When(s => s.Username is not null);
        RuleFor(s => s.Email).ValidEmail().When(s => s.Email is not null);
        RuleFor(s => s.Age).StudentAge().When(s => s.Age is not null);
    }
}
