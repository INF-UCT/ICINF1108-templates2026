using estudiantes_icinf.Data;
using estudiantes_icinf.Models;
using Microsoft.EntityFrameworkCore;

namespace estudiantes_icinf.Repositories;

public class PetsRepository(AppDbContext db)
{
    public Task<List<Pet>> FindAllByStudentAsync(Guid studentId) =>
        db.Pets.AsNoTracking()
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

    public Task<Pet?> FindByIdAsync(Guid id) =>
        db.Pets.FirstOrDefaultAsync(p => p.Id == id);

    public Task<Pet?> FindByStudentAndNameAsync(Guid studentId, string name) =>
        db.Pets.AsNoTracking().FirstOrDefaultAsync(p => p.StudentId == studentId && p.Name == name);

    public void Add(Pet pet) => db.Pets.Add(pet);

    public void Remove(Pet pet) => db.Pets.Remove(pet);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
