using BTG.Application.Interfaces;
using BTG.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace BTG.Infrastructure.Notifications;

public class ConsoleNotificacionService : INotificacionService
{
    private readonly ILogger<ConsoleNotificacionService> _logger;

    public ConsoleNotificacionService(ILogger<ConsoleNotificacionService> logger)
    {
        _logger = logger;
    }

    public Task EnviarSuscripcionAsync(Cliente c, Fondo f, decimal monto, CancellationToken ct)
    {
        _logger.LogInformation("Notificación enviada por {Canal} -> Cliente:{Cliente} Fondo:{Fondo} Monto:{Monto}",
            c.PreferenciaNotificacion, c.Nombre, f.Nombre, monto);

        return Task.CompletedTask;
    }
}
