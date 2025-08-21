using System.Security.Claims;
using BTG.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BTG.Api.Endpoints;

public static class TransaccionesEndpoints
{
    public static IEndpointRouteBuilder MapTransaccionesEndpoints(this IEndpointRouteBuilder app)
    {
        // Historial del cliente actual (rol cliente) o de cualquier cliente (rol admin)
        app.MapGet("/api/transacciones/historial/{clienteId:guid}", [Authorize] async (
            Guid clienteId,
            HttpContext http,
            ITransaccionRepository repo,
            CancellationToken ct) =>
        {
            var isAdmin = http.User.IsInRole("admin");
            if (!isAdmin)
            {
                var sub = http.User.FindFirst("sub")?.Value
                          ?? http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(sub) || !Guid.TryParse(sub, out var me) || me != clienteId)
                    return Results.Forbid();
            }

            var list = await repo.GetByClienteAsync(clienteId, ct);
            return Results.Ok(list);
        });

        return app;
    }
}
