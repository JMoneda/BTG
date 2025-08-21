using BTG.Application.Interfaces;
using BTG.Domain.Entities;
using MongoDB.Driver;

namespace BTG.Infrastructure.Persistence.Repositories;

public class ClienteRepository : IClienteRepository
{
    private readonly MongoContext _ctx;

    public ClienteRepository(MongoContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Cliente?> GetByIdAsync(Guid id, CancellationToken ct) =>
        await _ctx.Clientes.Find(c => c.Id == id).FirstOrDefaultAsync(ct);

    public Task AddAsync(Cliente cliente, CancellationToken ct) =>
        _ctx.Clientes.InsertOneAsync(cliente, cancellationToken: ct);

    public Task UpdateAsync(Cliente cliente, CancellationToken ct) =>
        _ctx.Clientes.ReplaceOneAsync(c => c.Id == cliente.Id, cliente, cancellationToken: ct);

    public async Task<Cliente?> GetByEmailAsync(string email, CancellationToken ct) =>
    await _ctx.Clientes.Find(c => c.Email == email).FirstOrDefaultAsync(ct);

}
