using estudiantes_icinf.Models;
using estudiantes_icinf.Services;
using estudiantes_icinf.Shared;

namespace estudiantes_icinf.Endpoints;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this WebApplication app)
    {
        // RequireAuthorization: todos los endpoints del grupo exigen sesión.
        var group = app.MapGroup("/api/students").WithTags("Students").RequireAuthorization();

        group.MapGet("/", async (StudentsService service) =>
        {
            var students = await service.FindAllAsync();
            return ApiResults.Ok(new { total = students.Count, items = students });
        });

        group.MapGet("/{id}", async (string id, StudentsService service) =>
            ApiResults.Ok(await service.FindByIdAsync(id)));

        group.MapPost("/", async (CreateStudentDto body, StudentsService service) =>
                ApiResults.Created(await service.CreateAsync(body)))
            .AddEndpointFilter<ValidationFilter<CreateStudentDto>>();

        group.MapPatch("/{id}", async (string id, UpdateStudentDto body, StudentsService service) =>
                ApiResults.Updated(await service.UpdateAsync(id, body)))
            .AddEndpointFilter<ValidationFilter<UpdateStudentDto>>();

        group.MapDelete("/{id}", async (string id, StudentsService service) =>
            ApiResults.Deleted(await service.DeleteAsync(id)));
    }
}
