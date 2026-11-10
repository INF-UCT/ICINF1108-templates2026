using estudiantes_icinf.Data;
using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;
using estudiantes_icinf.Shared;

namespace estudiantes_icinf.Services;

public class StudentsService(AppDbContext db, StudentsRepository students, PetsRepository pets)
{
    public Task<List<Student>> FindAllAsync() => students.FindAllAsync();

    public async Task<Student> FindByIdAsync(string id)
    {
        // Un id que no es UUID se trata igual que un id inexistente (404).
        var student = Guid.TryParse(id, out var guid) ? await students.FindByIdAsync(guid) : null;

        return student ?? throw ApiException.NotFound("Estudiante no encontrado");
    }

    public async Task<Student> CreateAsync(CreateStudentDto data)
    {
        // El estudiante y sus mascotas se insertan en una sola transacción: si
        // cualquier mascota falla, se hace rollback y no queda nada guardado.
        await using var transaction = await db.Database.BeginTransactionAsync();

        await AssertNoConflictsAsync(data.Email, data.Username);

        var student = new Student
        {
            Name = data.Name!,
            Username = data.Username!,
            Email = data.Email!,
            Age = data.Age!.Value,
        };

        students.Add(student);
        await students.SaveChangesAsync();

        foreach (var petData in data.Pets ?? [])
        {
            if (await pets.FindByStudentAndNameAsync(student.Id, petData.Name!) is not null)
            {
                // Al salir sin Commit, `await using` hace rollback de la transacción.
                throw ApiException.Conflict("name", "Ya tienes una mascota con ese nombre");
            }

            pets.Add(new Pet
            {
                StudentId = student.Id,
                Name = petData.Name!,
                Species = petData.Species!,
                Age = petData.Age,
            });

            await pets.SaveChangesAsync();
        }

        await transaction.CommitAsync();
        return student;
    }

    public async Task<Student> UpdateAsync(string id, UpdateStudentDto data)
    {
        var student = await FindByIdAsync(id);
        await AssertNoConflictsAsync(data.Email, data.Username, student.Id);

        // Los campos que no vienen en la petición no se modifican.
        student.Name = data.Name ?? student.Name;
        student.Username = data.Username ?? student.Username;
        student.Email = data.Email ?? student.Email;
        student.Age = data.Age ?? student.Age;

        await students.SaveChangesAsync();
        return student;
    }

    public async Task<Student> DeleteAsync(string id)
    {
        var student = await FindByIdAsync(id);

        // Las mascotas se eliminan en cascada (ON DELETE CASCADE).
        students.Remove(student);
        await students.SaveChangesAsync();

        return student;
    }

    private async Task AssertNoConflictsAsync(string? email, string? username, Guid? exceptId = null)
    {
        var conflicts = new Dictionary<string, FieldError>();

        if (email is not null)
        {
            var existing = await students.FindByEmailAsync(email);

            if (existing is not null && existing.Id != exceptId)
            {
                conflicts["email"] = new FieldError("El correo electrónico ya está en uso");
            }
        }

        if (username is not null)
        {
            var existing = await students.FindByUsernameAsync(username);

            if (existing is not null && existing.Id != exceptId)
            {
                conflicts["username"] = new FieldError("El nombre de usuario ya está en uso");
            }
        }

        if (conflicts.Count > 0)
        {
            throw ApiException.Conflict(conflicts);
        }
    }
}
