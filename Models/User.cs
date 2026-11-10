namespace estudiantes_icinf.Models;

// Usuarios que pueden iniciar sesión en la API (no confundir con Student,
// que es el recurso del CRUD).
public class User : ITimestamped
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

// Usuario sin el hash de la contraseña: lo único que sale de la API.
public record UserResponse(Guid Id, string Name, string Email, DateTime CreatedAt, DateTime UpdatedAt)
{
    public static UserResponse From(User user) =>
        new(user.Id, user.Name, user.Email, user.CreatedAt, user.UpdatedAt);
}
