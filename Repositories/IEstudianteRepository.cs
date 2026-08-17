using estudiantes_icinf.Models;

namespace estudiantes_icinf.Repositories;

public interface IEstudianteRepository
{
    Task<List<Estudiante>> GetAllAsync();
    Task<Estudiante?> GetByIdAsync(int id);
    Task<Estudiante> AddAsync(Estudiante estudiante);
    Task<Estudiante?> UpdateAsync(int id, Estudiante estudiante);
    Task<bool> DeleteAsync(int id);
}
