using Credits.Domain.Entities;

namespace Credits.Domain.Repositories;

public interface ICreditRepository
{
    Task AddAsync(Credit credit, CancellationToken ct = default);
    Task<IReadOnlyList<Credit>> GetByNfseNumberAsync(string nfseNumber, CancellationToken ct = default);
    Task<Credit?> GetByCreditNumberAsync(string creditNumber, CancellationToken ct = default);
    Task<bool> ExistsByCreditNumberAsync(string creditNumber, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
