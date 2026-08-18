using estudiantes_icinf.Endpoints;
using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;
using estudiantes_icinf.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<IEstudianteRepository, JsonEstudianteRepository>();
builder.Services.AddScoped<IValidator<Estudiante>, EstudianteValidator>();

builder.Services.AddSingleton<IProfesorRepository, JsonProfesorRepository>();
builder.Services.AddScoped<IValidator<Profesor>, ProfesorValidator>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapEstudianteEndpoints();
app.MapProfesorEndpoints();

app.Run();
