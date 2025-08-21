using System.Security.Claims;
using BTG.Application.Interfaces;
using BTG.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using BTG.Application.DTOs;

namespace BTG.Api.Endpoints;

public static class ClientesEndpoints
{
    public static IEndpointRouteBuilder MapClientesEndpoints(this IEndpointRouteBuilder app)
    {
        // Crear perfil de cliente (solo quien tenga rol "cliente")
        app.MapPost("/api/clientes", [Authorize(Roles = "cliente")] async (
    CrearClienteRequest request,
    HttpContext http,
    IClienteRepository repo,
    CancellationToken ct) =>
        {
            //Validaciones de entrada(pueden ser mas según defina el alcance con negocio)
            if (string.IsNullOrWhiteSpace(request.Nombre) || string.IsNullOrWhiteSpace(request.Email))
                return Results.BadRequest(new { error = "Nombre y Email son obligatorios" });

            if (!request.Email.Contains("@"))
                return Results.BadRequest(new { error = "Email inválido" });

            if (request.Saldo < 0)
                return Results.BadRequest(new { error = "Saldo no puede ser negativo" });

            //Sacar el userId del token
            var sub = http.User.FindFirst("sub")?.Value
                      ?? http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var clienteId))
                return Results.Unauthorized();

            //Verificar si ya existe
            var existente = await repo.GetByIdAsync(clienteId, ct);
            if (existente is not null)
                return Results.BadRequest(new { error = "Cliente ya existe" });

            //Crear nuevo cliente
            var cliente = new Cliente
            {
                Id = clienteId,
                Nombre = request.Nombre,
                Email = request.Email,
                Telefono = request.Telefono,
                Saldo = request.Saldo,
                PreferenciaNotificacion = request.PreferenciaNotificacion
            };

            await repo.AddAsync(cliente, ct);

            return Results.Created($"/api/clientes/{cliente.Id}", cliente);
        });


        //Obtener cliente por id (admin o el mismo cliente)
        app.MapGet("/api/clientes/{id:guid}", [Authorize] async (
            Guid id,
            HttpContext http,
            IClienteRepository repo,
            CancellationToken ct) =>
        {
            var c = await repo.GetByIdAsync(id, ct);
            if (c is null) return Results.NotFound();

        //Si no es admin, debe ser el mismo usuario
            var isAdmin = http.User.IsInRole("admin");
            if (!isAdmin)
            {
                var sub = http.User.FindFirst("sub")?.Value
                          ?? http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var me) || me != id)
                    return Results.Forbid();
            }

            return Results.Ok(c);
        });

        return app;
    }
}
