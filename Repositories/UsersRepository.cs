using estudiantes_icinf.Data;
using estudiantes_icinf.Models;
using Microsoft.EntityFrameworkCore;

namespace estudiantes_icinf.Repositories;

public class UsersRepository(AppDbContext db)
{
    public Task<User?> FindByIdAsync(Guid id) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    public Task<User?> FindByEmailAsync(string email) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User> CreateAsync(User user)
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }
}
