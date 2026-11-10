namespace estudiantes_icinf.Models;

// Sesión guardada en Postgres. Al navegador solo llega una cookie cifrada
// con el Id; el contenido (usuario, expiración) queda en la base de datos.
public class UserSession
{
    public string Id { get; set; } = string.Empty;
    public byte[] Value { get; set; } = [];
    public DateTimeOffset? ExpiresAt { get; set; }
}
