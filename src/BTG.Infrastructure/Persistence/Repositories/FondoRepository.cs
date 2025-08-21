using BTG.Application.Interfaces;
using BTG.Domain.Entities;
using MongoDB.Driver;

namespace BTG.Infrastructure.Persistence.Repositories;

public class FondoRepository : IFondoRepository
{
    private readonly MongoContext _ctx;

    public FondoRepository(MongoContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<Fondo?> GetByIdAsync(string id, CancellationToken ct) =>
        await _ctx.Fondos.Find(f => f.Id == id).FirstOrDefaultAsync(ct);

    public async Task<List<Fondo>> GetAllAsync(CancellationToken ct) =>
        await _ctx.Fondos.Find(_ => true).ToListAsync(ct);

    public async Task SeedIfEmptyAsync(IEnumerable<Fondo> fondos, CancellationToken ct)
    {
        var count = await _ctx.Fondos.CountDocumentsAsync(_ => true, cancellationToken: ct);
        if (count == 0)
        {
            await _ctx.Fondos.InsertManyAsync(fondos, cancellationToken: ct);
        }
    }
}
