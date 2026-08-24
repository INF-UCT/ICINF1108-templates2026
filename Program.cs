using estudiantes_icinf.Endpoints;
using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;
using estudiantes_icinf.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IStudentRepository, JsonStudentRepository>();
builder.Services.AddScoped<IValidator<CreateStudentDto>, CreateStudentValidator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c => c.RoutePrefix = "docs");

app.UseHttpsRedirection();

app.MapStudentEndpoints();

app.Run();
