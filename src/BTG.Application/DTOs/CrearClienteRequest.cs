using BTG.Domain.Entities;

public record CrearClienteRequest(
    string Nombre,
    string Email,
    string Telefono,
    decimal Saldo,
    PreferenciaNotificacion PreferenciaNotificacion);
