using estudiantes_icinf.Models;

namespace estudiantes_icinf.Repositories;

public interface IProfesorRepository
{
    Task<List<Profesor>> GetAllAsync();
    Task<Profesor?> GetByIdAsync(int id);
    Task<Profesor> AddAsync(Profesor Profesor);
    Task<Profesor?> UpdateAsync(int id, Profesor Profesor);
    Task<bool> DeleteAsync(int id);
}
