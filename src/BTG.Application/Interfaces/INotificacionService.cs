using BTG.Domain.Entities;

namespace BTG.Application.Interfaces;

public interface INotificacionService
{
    Task EnviarSuscripcionAsync(Cliente c, Fondo f, decimal monto, CancellationToken ct);
}
