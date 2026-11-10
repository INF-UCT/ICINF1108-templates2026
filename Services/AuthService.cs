using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;
using estudiantes_icinf.Shared;
using Microsoft.AspNetCore.Identity;

namespace estudiantes_icinf.Services;

public class AuthService(UsersRepository users, IPasswordHasher<User> passwordHasher)
{
    public async Task<UserResponse> RegisterAsync(RegisterDto data)
    {
        if (await users.FindByEmailAsync(data.Email!) is not null)
        {
            throw ApiException.Conflict("email", "El correo electrónico ya está registrado");
        }

        var user = new User { Name = data.Name!, Email = data.Email! };

        // PasswordHasher de ASP.NET Identity: PBKDF2 con sal aleatoria.
        // La contraseña nunca se guarda en texto plano.
        user.PasswordHash = passwordHasher.HashPassword(user, data.Password!);

        return UserResponse.From(await users.CreateAsync(user));
    }

    public async Task<UserResponse> LoginAsync(LoginDto data)
    {
        var user = await users.FindByEmailAsync(data.Email!);

        // Mismo mensaje si el correo no existe o la contraseña es incorrecta,
        // para no revelar qué correos están registrados.
        if (user is null ||
            passwordHasher.VerifyHashedPassword(user, user.PasswordHash, data.Password!) ==
            PasswordVerificationResult.Failed)
        {
            throw ApiException.Unauthorized("Correo o contraseña incorrectos");
        }

        return UserResponse.From(user);
    }

    public async Task<UserResponse?> FindByIdAsync(Guid id)
    {
        var user = await users.FindByIdAsync(id);
        return user is null ? null : UserResponse.From(user);
    }
}
