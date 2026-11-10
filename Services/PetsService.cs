using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;
using estudiantes_icinf.Shared;

namespace estudiantes_icinf.Services;

public class PetsService(PetsRepository pets, StudentsService studentsService)
{
    public async Task<List<Pet>> FindAllForStudentAsync(string studentId)
    {
        var student = await studentsService.FindByIdAsync(studentId);
        return await pets.FindAllByStudentAsync(student.Id);
    }

    public async Task<Pet> CreateAsync(string studentId, CreatePetDto data)
    {
        var student = await studentsService.FindByIdAsync(studentId);
        await AssertNoConflictsAsync(student.Id, data.Name);

        var pet = new Pet
        {
            StudentId = student.Id,
            Name = data.Name!,
            Species = data.Species!,
            Age = data.Age,
        };

        pets.Add(pet);
        await pets.SaveChangesAsync();
        return pet;
    }

    public async Task<Pet> UpdateAsync(string studentId, string petId, UpdatePetDto data)
    {
        var pet = await FindOwnedAsync(studentId, petId);
        await AssertNoConflictsAsync(pet.StudentId, data.Name, pet.Id);

        // Los campos que no vienen en la petición no se modifican.
        pet.Name = data.Name ?? pet.Name;
        pet.Species = data.Species ?? pet.Species;
        pet.Age = data.Age ?? pet.Age;

        await pets.SaveChangesAsync();
        return pet;
    }

    public async Task<Pet> DeleteAsync(string studentId, string petId)
    {
        var pet = await FindOwnedAsync(studentId, petId);

        pets.Remove(pet);
        await pets.SaveChangesAsync();

        return pet;
    }

    private async Task<Pet> FindOwnedAsync(string studentId, string petId)
    {
        var student = await studentsService.FindByIdAsync(studentId);
        var pet = Guid.TryParse(petId, out var guid) ? await pets.FindByIdAsync(guid) : null;

        if (pet is null || pet.StudentId != student.Id)
        {
            throw ApiException.NotFound("Mascota no encontrada");
        }

        return pet;
    }

    private async Task AssertNoConflictsAsync(Guid studentId, string? name, Guid? exceptId = null)
    {
        if (name is null) return;

        var existing = await pets.FindByStudentAndNameAsync(studentId, name);

        if (existing is not null && existing.Id != exceptId)
        {
            throw ApiException.Conflict("name", "Ya tienes una mascota con ese nombre");
        }
    }
}
