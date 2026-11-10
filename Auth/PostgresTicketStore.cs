using estudiantes_icinf.Data;
using estudiantes_icinf.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace estudiantes_icinf.Auth;

// Guarda las sesiones en la tabla `user_sessions` de Postgres. Así la cookie
// solo lleva un id de sesión (cifrado), y cerrar sesión la borra de verdad
// en el servidor.
public class PostgresTicketStore(IServiceScopeFactory scopeFactory) : ITicketStore
{
    public async Task<string> StoreAsync(AuthenticationTicket ticket)
    {
        var key = Guid.NewGuid().ToString("N");

        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        db.UserSessions.Add(new UserSession
        {
            Id = key,
            Value = TicketSerializer.Default.Serialize(ticket),
            ExpiresAt = ticket.Properties.ExpiresUtc?.ToUniversalTime(),
        });

        await db.SaveChangesAsync();
        return key;
    }

    public async Task RenewAsync(string key, AuthenticationTicket ticket)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var session = await db.UserSessions.FindAsync(key);
        if (session is null) return;

        session.Value = TicketSerializer.Default.Serialize(ticket);
        session.ExpiresAt = ticket.Properties.ExpiresUtc?.ToUniversalTime();
        await db.SaveChangesAsync();
    }

    public async Task<AuthenticationTicket?> RetrieveAsync(string key)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var session = await db.UserSessions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == key);
        if (session is null) return null;

        // Sesión vencida: se elimina y se trata como inexistente.
        if (session.ExpiresAt is not null && session.ExpiresAt < DateTimeOffset.UtcNow)
        {
            await RemoveAsync(key);
            return null;
        }

        return TicketSerializer.Default.Deserialize(session.Value);
    }

    public async Task RemoveAsync(string key)
    {
        await using var scope = scopeFactory.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await db.UserSessions.Where(s => s.Id == key).ExecuteDeleteAsync();
    }
}
