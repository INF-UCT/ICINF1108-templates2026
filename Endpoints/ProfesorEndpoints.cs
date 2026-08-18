using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;
using FluentValidation;

namespace estudiantes_icinf.Endpoints;

public static class ProfesorEndpoints
{
    public static void MapProfesorEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/profesores").WithTags("Profesores");

        group.MapGet("/", async (IProfesorRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/{id:int}", async (int id, IProfesorRepository repo) =>
        {
            var profesor = await repo.GetByIdAsync(id);
            return profesor is not null ? Results.Ok(profesor) : Results.NotFound();
        });

        group.MapPost("/", async (Profesor profesor, IValidator<Profesor> validator, IProfesorRepository repo) =>
        {
            var resultado = await validator.ValidateAsync(profesor);
            if (!resultado.IsValid)
            {
                return Results.ValidationProblem(resultado.ToDictionary());
            }

            var creado = await repo.AddAsync(profesor);
            return Results.Created($"/api/profesores/{creado.Id}", creado);
        });

        group.MapPut("/{id:int}", async (int id, Profesor profesor, IValidator<Profesor> validator, IProfesorRepository repo) =>
        {
            var resultado = await validator.ValidateAsync(profesor);
            if (!resultado.IsValid)
            {
                return Results.ValidationProblem(resultado.ToDictionary());
            }

            var actualizado = await repo.UpdateAsync(id, profesor);
            return actualizado is not null ? Results.Ok(actualizado) : Results.NotFound();
        });

        group.MapDelete("/{id:int}", async (int id, IProfesorRepository repo) =>
        {
            var eliminado = await repo.DeleteAsync(id);
            return eliminado ? Results.NoContent() : Results.NotFound();
        });
    }
}
