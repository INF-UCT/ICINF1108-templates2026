using estudiantes_icinf.Models;
using FluentValidation;

namespace estudiantes_icinf.Validators;

public class CreatePetValidator : AbstractValidator<CreatePetDto>
{
    public CreatePetValidator()
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("El nombre no puede estar vacío").PetText("El nombre");
        RuleFor(p => p.Species).NotEmpty().WithMessage("La especie no puede estar vacía").PetText("La especie");
        RuleFor(p => p.Age).PetAge().When(p => p.Age is not null);
    }
}

public class UpdatePetValidator : AbstractValidator<UpdatePetDto>
{
    public UpdatePetValidator()
    {
        RuleFor(p => p.Name).PetText("El nombre").When(p => p.Name is not null);
        RuleFor(p => p.Species).PetText("La especie").When(p => p.Species is not null);
        RuleFor(p => p.Age).PetAge().When(p => p.Age is not null);
    }
}
