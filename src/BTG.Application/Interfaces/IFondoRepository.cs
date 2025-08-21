using BTG.Domain.Entities;

namespace BTG.Application.Interfaces;

public interface IFondoRepository
{
    Task<Fondo?> GetByIdAsync(string id, CancellationToken ct);
    Task<List<Fondo>> GetAllAsync(CancellationToken ct);
    Task SeedIfEmptyAsync(IEnumerable<Fondo> fondos, CancellationToken ct);
}
