using estudiantes_icinf.Endpoints;
using estudiantes_icinf.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IEstudianteRepository, JsonEstudianteRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapEstudianteEndpoints();

app.Run();
