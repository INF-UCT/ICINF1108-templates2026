using System.Text.Json.Serialization;

namespace estudiantes_icinf.Models;

public class Pet : ITimestamped
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public int? Age { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Relación con el estudiante dueño. No se incluye en las respuestas JSON.
    [JsonIgnore]
    public Student? Student { get; set; }
}
