using BTG.Application.DTOs;
using BTG.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BTG.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {   //Registro
        app.MapPost("/auth/register", async (RegisterRequest req, IAuthService svc, CancellationToken ct) =>
        {
            var res = await svc.RegisterAsync(req, ct);
            return Results.Ok(res);
        }).AllowAnonymous();
        //Login
        app.MapPost("/auth/login", async (LoginRequest req, IAuthService svc, CancellationToken ct) =>
        {
            var res = await svc.LoginAsync(req, ct);
            return Results.Ok(res);
        }).AllowAnonymous();
        //Refresh Token
        app.MapPost("/auth/refresh", async (RefreshRequest req, IAuthService svc, CancellationToken ct) =>
        {
            var res = await svc.RefreshAsync(req, ct);
            return Results.Ok(res);
        }).AllowAnonymous();
        // Revoke Token
        app.MapPost("/auth/revoke", async (string? refreshToken, HttpContext ctx, IAuthService svc, CancellationToken ct) =>
        {
            var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException();
            await svc.RevokeAsync(userId, refreshToken, ct);
            return Results.NoContent();
        }).RequireAuthorization();
        // Ping para verificar autenticación
        app.MapGet("/admin/ping", () => Results.Ok("pong admin"))
           .RequireAuthorization("OnlyAdmins");

        return app;
    }
}
