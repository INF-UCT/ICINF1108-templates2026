namespace estudiantes_icinf.Models;

// Entidades con fechas de creación y actualización que AppDbContext completa
// automáticamente al guardar.
public interface ITimestamped
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}
