using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;

namespace estudiantes_icinf.Endpoints;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/students").WithTags("Students");

        group.MapGet("/", async (IStudentRepository repo) =>
            Results.Ok(await repo.GetAllAsync()));

        group.MapGet("/{id:guid}", async (Guid id, IStudentRepository repo) =>
            Results.Ok(await repo.GetByIdAsync(id)));

        group.MapPost("/", async (CreateStudentDto dto, IStudentRepository repo) =>
        {
            if (await repo.GetByEmailAsync(dto.Email) is not null)
            {
                return Results.Conflict(new { error = "El email ya esta registrado." });
            }

            var creado = await repo.AddAsync(dto);
            return Results.Created($"/api/students/{creado.Id}", creado);
        });

        group.MapPatch("/{id:guid}", async (Guid id, UpdateStudentDto dto, IStudentRepository repo) =>
        {
            if (dto.Email is not null && await repo.GetByEmailAsync(dto.Email, id) is not null)
            {
                return Results.Conflict(new { error = "El email ya esta registrado." });
            }

            var actualizado = await repo.UpdateAsync(id, dto);
            return Results.Ok(actualizado);
        });

        group.MapDelete("/{id:guid}", async (Guid id, IStudentRepository repo) =>
        {
            await repo.DeleteAsync(id);
            return Results.NoContent();
        });
    }
}
