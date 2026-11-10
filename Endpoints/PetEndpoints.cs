using estudiantes_icinf.Models;
using estudiantes_icinf.Services;
using estudiantes_icinf.Shared;

namespace estudiantes_icinf.Endpoints;

public static class PetEndpoints
{
    public static void MapPetEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/students/{studentId}/pets").WithTags("Pets").RequireAuthorization();

        group.MapGet("/", async (string studentId, PetsService service) =>
        {
            var pets = await service.FindAllForStudentAsync(studentId);
            return ApiResults.Ok(new { total = pets.Count, items = pets });
        });

        group.MapPost("/", async (string studentId, CreatePetDto body, PetsService service) =>
                ApiResults.Created(await service.CreateAsync(studentId, body)))
            .AddEndpointFilter<ValidationFilter<CreatePetDto>>();

        group.MapPatch("/{petId}", async (string studentId, string petId, UpdatePetDto body, PetsService service) =>
                ApiResults.Updated(await service.UpdateAsync(studentId, petId, body)))
            .AddEndpointFilter<ValidationFilter<UpdatePetDto>>();

        group.MapDelete("/{petId}", async (string studentId, string petId, PetsService service) =>
            ApiResults.Deleted(await service.DeleteAsync(studentId, petId)));
    }
}
