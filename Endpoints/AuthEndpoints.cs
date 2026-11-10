using System.Security.Claims;
using estudiantes_icinf.Models;
using estudiantes_icinf.Services;
using estudiantes_icinf.Shared;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace estudiantes_icinf.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Auth");

        // Crea la cuenta e inicia sesión de inmediato.
        group.MapPost("/register", async (RegisterDto body, AuthService service, HttpContext context) =>
            {
                var user = await service.RegisterAsync(body);
                await StartSessionAsync(context, user);
                return ApiResults.Created(user);
            })
            .AddEndpointFilter<ValidationFilter<RegisterDto>>();

        group.MapPost("/login", async (LoginDto body, AuthService service, HttpContext context) =>
            {
                var user = await service.LoginAsync(body);
                await StartSessionAsync(context, user);
                return ApiResults.Ok(user);
            })
            .AddEndpointFilter<ValidationFilter<LoginDto>>();

        // Borra la sesión de Postgres y la cookie del navegador.
        group.MapPost("/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return ApiResults.Ok(null);
        });

        group.MapGet("/me", async (ClaimsPrincipal principal, AuthService service, HttpContext context) =>
            {
                var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var user = await service.FindByIdAsync(userId);

                // La sesión apunta a un usuario que ya no existe.
                if (user is null)
                {
                    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    throw ApiException.Unauthorized("Debes iniciar sesión para acceder a este recurso");
                }

                return ApiResults.Ok(user);
            })
            .RequireAuthorization();
    }

    // Si ya había una sesión, SignInAsync la reemplaza por una nueva
    // (id de sesión nuevo), lo que evita ataques de fijación de sesión.
    private static Task StartSessionAsync(HttpContext context, UserResponse user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        return context.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });
    }
}
