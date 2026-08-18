using System.Text.Json;
using estudiantes_icinf.Models;

namespace estudiantes_icinf.Repositories;

public class JsonProfesorRepository : IProfesorRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonProfesorRepository(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Data", "profesores.json");
    }

    public async Task<List<Profesor>> GetAllAsync()
    {
        await _lock.WaitAsync();
        try
        {
            return await ReadAllAsync();
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Profesor?> GetByIdAsync(int id)
    {
        var Profesors = await GetAllAsync();
        return Profesors.FirstOrDefault(e => e.Id == id);
    }

    public async Task<Profesor> AddAsync(Profesor Profesor)
    {
        await _lock.WaitAsync();
        try
        {
            var Profesors = await ReadAllAsync();
            Profesor.Id = Profesors.Count == 0 ? 1 : Profesors.Max(e => e.Id) + 1;
            Profesors.Add(Profesor);
            await WriteAllAsync(Profesors);
            return Profesor;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Profesor?> UpdateAsync(int id, Profesor Profesor)
    {
        await _lock.WaitAsync();
        try
        {
            var Profesors = await ReadAllAsync();
            var existente = Profesors.FirstOrDefault(e => e.Id == id);
            if (existente is null)
            {
                return null;
            }

            existente.Nombre = Profesor.Nombre;
            existente.FechaIngreso = Profesor.FechaIngreso;
            existente.Email = Profesor.Email;


            await WriteAllAsync(Profesors);
            return existente;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        await _lock.WaitAsync();
        try
        {
            var Profesors = await ReadAllAsync();
            var eliminados = Profesors.RemoveAll(e => e.Id == id);
            if (eliminados == 0)
            {
                return false;
            }

            await WriteAllAsync(Profesors);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<Profesor>> ReadAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        var Profesors = await JsonSerializer.DeserializeAsync<List<Profesor>>(stream, SerializerOptions);
        return Profesors ?? [];
    }

    private async Task WriteAllAsync(List<Profesor> Profesors)
    {
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, Profesors, SerializerOptions);
    }
}
