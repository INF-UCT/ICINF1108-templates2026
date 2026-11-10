using estudiantes_icinf.Auth;
using estudiantes_icinf.Data;
using estudiantes_icinf.Endpoints;
using estudiantes_icinf.Models;
using estudiantes_icinf.Repositories;
using estudiantes_icinf.Services;
using estudiantes_icinf.Shared;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Base de datos --------------------------------------------------------

// En Docker se sobreescribe con la variable ConnectionStrings__Default.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// --- Capas de la aplicación -----------------------------------------------

builder.Services.AddScoped<StudentsRepository>();
builder.Services.AddScoped<PetsRepository>();
builder.Services.AddScoped<UsersRepository>();
builder.Services.AddScoped<StudentsService>();
builder.Services.AddScoped<PetsService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// --- Sesiones -------------------------------------------------------------

var sessionMaxAge = TimeSpan.FromMinutes(builder.Configuration.GetValue("Session:MaxAgeMinutes", 480));
var cookieSecure = builder.Configuration.GetValue("Session:CookieSecure", false);

// Las claves que cifran la cookie se guardan en Postgres para que las
// sesiones sobrevivan a reinicios de la API.
builder.Services.AddDataProtection()
    .SetApplicationName("icinf1108")
    .PersistKeysToDbContext<AppDbContext>();

builder.Services.AddSingleton<PostgresTicketStore>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "sid";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = cookieSecure ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = sessionMaxAge;
        options.SlidingExpiration = false;

        // Es una API: sin sesión se responde 401 con el envelope, no una
        // redirección a una página de login.
        options.Events.OnRedirectToLogin = context => WriteErrorAsync(
            context.Response, StatusCodes.Status401Unauthorized, "Debes iniciar sesión para acceder a este recurso");
        options.Events.OnRedirectToAccessDenied = context => WriteErrorAsync(
            context.Response, StatusCodes.Status403Forbidden, "No tienes permiso para acceder a este recurso");
    });

// La sesión se guarda en Postgres (tabla user_sessions) y no dentro de la cookie.
builder.Services
    .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
    .Configure<PostgresTicketStore>((options, store) => options.SessionStore = store);

builder.Services.AddAuthorization();

// --- HTTP -----------------------------------------------------------------

builder.Services.AddExceptionHandler<AppExceptionHandler>();
builder.Services.AddProblemDetails();

// JSON inválido en el cuerpo: se lanza BadHttpRequestException para que
// AppExceptionHandler responda con el envelope (también fuera de Development).
builder.Services.Configure<RouteHandlerOptions>(options => options.ThrowOnBadRequest = true);
builder.Services.Configure<JsonOptions>(options => options.SerializerOptions.PropertyNameCaseInsensitive = true);

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Aplica las migraciones pendientes al iniciar.
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseExceptionHandler();

app.UseSwagger();
app.UseSwaggerUI(c => c.RoutePrefix = "docs");

// Frontend estático (wwwroot/): index.html, pets.html y login.html.
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapStudentEndpoints();
app.MapPetEndpoints();

// Cualquier otra ruta bajo /api responde 404 con el envelope.
app.Map("/api/{**path}", (HttpContext context) =>
    ApiResults.Fail(StatusCodes.Status404NotFound, $"Cannot {context.Request.Method} {context.Request.Path}"));

app.Run();

static Task WriteErrorAsync(HttpResponse response, int statusCode, string message)
{
    response.StatusCode = statusCode;
    return response.WriteAsJsonAsync(ApiResponse.Fail(statusCode, message));
}
