using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;

namespace estudiantes_icinf.Endpoints;

public static class EstudianteEndpoints
{
    public static void MapEstudianteEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/estudiantes").WithTags("Estudiantes");

        group.MapGet("/", async (IEstudianteRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, IEstudianteRepository repo) =>
        {
            var estudiante = await repo.GetByIdAsync(id);
            return estudiante is not null ? Results.Ok(estudiante) : Results.NotFound();
        });

        group.MapPost("/", async (Estudiante estudiante, IEstudianteRepository repo) =>
        {
            var creado = await repo.AddAsync(estudiante);
            return Results.Created($"/api/estudiantes/{creado.Id}", creado);
        });

        group.MapPut("/{id:int}", async (int id, Estudiante estudiante, IEstudianteRepository repo) =>
        {
            var actualizado = await repo.UpdateAsync(id, estudiante);
            return actualizado is not null ? Results.Ok(actualizado) : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IEstudianteRepository repo) =>
        {
            var eliminado = await repo.DeleteAsync(id);
            return eliminado ? Results.NoContent() : Results.NotFound();
        });
    }
}
