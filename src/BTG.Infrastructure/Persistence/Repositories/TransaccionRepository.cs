using BTG.Application.Interfaces;
using BTG.Domain.Entities;
using MongoDB.Driver;

namespace BTG.Infrastructure.Persistence.Repositories;

public class TransaccionRepository : ITransaccionRepository
{
    private readonly MongoContext _ctx;

    public TransaccionRepository(MongoContext ctx)
    {
        _ctx = ctx;
    }

    public Task AddAsync(Transaccion t, CancellationToken ct) =>
        _ctx.Transacciones.InsertOneAsync(t, cancellationToken: ct);

    public async Task<List<Transaccion>> GetByClienteAsync(Guid clienteId, CancellationToken ct) =>
        await _ctx.Transacciones
            .Find(t => t.ClienteId == clienteId)
            .SortByDescending(t => t.Fecha)
            .ToListAsync(ct);
}
