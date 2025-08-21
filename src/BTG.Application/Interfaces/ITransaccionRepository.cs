using BTG.Domain.Entities;

namespace BTG.Application.Interfaces;

public interface ITransaccionRepository
{
    Task AddAsync(Transaccion t, CancellationToken ct);
    Task<List<Transaccion>> GetByClienteAsync(Guid clienteId, CancellationToken ct);
}
