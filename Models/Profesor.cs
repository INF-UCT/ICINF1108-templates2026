namespace estudiantes_icinf.Models;

public class Profesor
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;

    public int FechaIngreso { get; set; } = 0;
    public string Email { get; set; } = string.Empty;

}
