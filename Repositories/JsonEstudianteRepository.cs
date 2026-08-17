using System.Text.Json;
using estudiantes_icinf.Models;

namespace estudiantes_icinf.Repositories;

public class JsonEstudianteRepository : IEstudianteRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonEstudianteRepository(IWebHostEnvironment env)
    {
        _filePath = Path.Combine(env.ContentRootPath, "Data", "estudiantes.json");
    }

    public async Task<List<Estudiante>> GetAllAsync()
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

    public async Task<Estudiante?> GetByIdAsync(int id)
    {
        var estudiantes = await GetAllAsync();
        return estudiantes.FirstOrDefault(e => e.Id == id);
    }

    public async Task<Estudiante> AddAsync(Estudiante estudiante)
    {
        await _lock.WaitAsync();
        try
        {
            var estudiantes = await ReadAllAsync();
            estudiante.Id = estudiantes.Count == 0 ? 1 : estudiantes.Max(e => e.Id) + 1;
            estudiantes.Add(estudiante);
            await WriteAllAsync(estudiantes);
            return estudiante;
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task<Estudiante?> UpdateAsync(int id, Estudiante estudiante)
    {
        await _lock.WaitAsync();
        try
        {
            var estudiantes = await ReadAllAsync();
            var existente = estudiantes.FirstOrDefault(e => e.Id == id);
            if (existente is null)
            {
                return null;
            }

            existente.Nombre = estudiante.Nombre;
            existente.Apellido = estudiante.Apellido;
            existente.Email = estudiante.Email;
            existente.Carrera = estudiante.Carrera;

            await WriteAllAsync(estudiantes);
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
            var estudiantes = await ReadAllAsync();
            var eliminados = estudiantes.RemoveAll(e => e.Id == id);
            if (eliminados == 0)
            {
                return false;
            }

            await WriteAllAsync(estudiantes);
            return true;
        }
        finally
        {
            _lock.Release();
        }
    }

    private async Task<List<Estudiante>> ReadAllAsync()
    {
        if (!File.Exists(_filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(_filePath);
        var estudiantes = await JsonSerializer.DeserializeAsync<List<Estudiante>>(stream, SerializerOptions);
        return estudiantes ?? [];
    }

    private async Task WriteAllAsync(List<Estudiante> estudiantes)
    {
        await using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, estudiantes, SerializerOptions);
    }
}
