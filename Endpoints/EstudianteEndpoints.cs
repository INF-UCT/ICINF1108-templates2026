using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;
using FluentValidation;

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

        group.MapPost("/", async (Estudiante estudiante, IValidator<Estudiante> validator, IEstudianteRepository repo) =>
        {
            var resultado = await validator.ValidateAsync(estudiante);
            if (!resultado.IsValid)
            {
                return Results.ValidationProblem(resultado.ToDictionary());
            }

            var creado = await repo.AddAsync(estudiante);
            return Results.Created($"/api/estudiantes/{creado.Id}", creado);
        });

        group.MapPut("/{id:int}", async (int id, Estudiante estudiante, IValidator<Estudiante> validator, IEstudianteRepository repo) =>
        {
            var resultado = await validator.ValidateAsync(estudiante);
            if (!resultado.IsValid)
            {
                return Results.ValidationProblem(resultado.ToDictionary());
            }

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
