using BTG.Domain.Entities;

namespace BTG.Application.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<Cliente?> GetByEmailAsync(string email, CancellationToken ct);
        Task AddAsync(Cliente cliente, CancellationToken ct);
        Task UpdateAsync(Cliente cliente, CancellationToken ct);
    }
}
