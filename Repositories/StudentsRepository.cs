using estudiantes_icinf.Data;
using estudiantes_icinf.Models;
using Microsoft.EntityFrameworkCore;

namespace estudiantes_icinf.Repositories;

public class StudentsRepository(AppDbContext db)
{
    public Task<List<Student>> FindAllAsync() =>
        db.Students.AsNoTracking().OrderByDescending(s => s.CreatedAt).ToListAsync();

    public Task<Student?> FindByIdAsync(Guid id) =>
        db.Students.FirstOrDefaultAsync(s => s.Id == id);

    public Task<Student?> FindByEmailAsync(string email) =>
        db.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Email == email);

    public Task<Student?> FindByUsernameAsync(string username) =>
        db.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Username == username);

    public void Add(Student student) => db.Students.Add(student);

    public void Remove(Student student) => db.Students.Remove(student);

    public Task SaveChangesAsync() => db.SaveChangesAsync();
}
