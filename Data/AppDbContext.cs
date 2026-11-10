using estudiantes_icinf.Models;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace estudiantes_icinf.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options), IDataProtectionKeyContext
{
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    // Claves con las que ASP.NET cifra la cookie de sesión. Se guardan en
    // Postgres para que las sesiones sigan siendo válidas tras reiniciar la API.
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(e =>
        {
            e.ToTable("students");
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasColumnName("id");
            e.Property(s => s.Name).HasColumnName("name").HasMaxLength(100);
            e.Property(s => s.Username).HasColumnName("username").HasMaxLength(30);
            e.Property(s => s.Email).HasColumnName("email").HasMaxLength(255);
            e.Property(s => s.Age).HasColumnName("age");
            e.Property(s => s.CreatedAt).HasColumnName("created_at");
            e.Property(s => s.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(s => s.Username).IsUnique();
            e.HasIndex(s => s.Email).IsUnique();
        });

        modelBuilder.Entity<Pet>(e =>
        {
            e.ToTable("pets");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasColumnName("id");
            e.Property(p => p.StudentId).HasColumnName("student_id");
            e.Property(p => p.Name).HasColumnName("name").HasMaxLength(50);
            e.Property(p => p.Species).HasColumnName("species").HasMaxLength(50);
            e.Property(p => p.Age).HasColumnName("age");
            e.Property(p => p.CreatedAt).HasColumnName("created_at");
            e.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(p => new { p.StudentId, p.Name }).IsUnique();

            // Al eliminar un estudiante, Postgres elimina sus mascotas (ON DELETE CASCADE).
            e.HasOne(p => p.Student)
                .WithMany()
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasColumnName("id");
            e.Property(u => u.Name).HasColumnName("name").HasMaxLength(100);
            e.Property(u => u.Email).HasColumnName("email").HasMaxLength(255);
            e.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(255);
            e.Property(u => u.CreatedAt).HasColumnName("created_at");
            e.Property(u => u.UpdatedAt).HasColumnName("updated_at");
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<UserSession>(e =>
        {
            e.ToTable("user_sessions");
            e.HasKey(s => s.Id);
            e.Property(s => s.Id).HasColumnName("id").HasMaxLength(64);
            e.Property(s => s.Value).HasColumnName("value");
            e.Property(s => s.ExpiresAt).HasColumnName("expires_at");
            e.HasIndex(s => s.ExpiresAt);
        });

        modelBuilder.Entity<DataProtectionKey>(e =>
        {
            e.ToTable("data_protection_keys");
            e.Property(k => k.Id).HasColumnName("id");
            e.Property(k => k.FriendlyName).HasColumnName("friendly_name");
            e.Property(k => k.Xml).HasColumnName("xml");
        });
    }

    // Completa CreatedAt/UpdatedAt antes de guardar (como @CreateDateColumn /
    // @updatedAt en otros ORMs).
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<ITimestamped>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
