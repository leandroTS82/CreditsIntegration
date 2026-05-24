using Credits.Domain.Entities;
using Credits.Domain.Repositories;
using Credits.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Credits.Infrastructure.Repositories;

public sealed class CreditRepository(AppDbContext context) : ICreditRepository
{
    public async Task AddAsync(Credit credit, CancellationToken ct = default)
        => await context.Credits.AddAsync(credit, ct);

    public async Task<IReadOnlyList<Credit>> GetByNfseNumberAsync(string nfseNumber, CancellationToken ct = default)
        => await context.Credits
            .AsNoTracking()
            .Where(c => c.NfseNumber == nfseNumber)
            .ToListAsync(ct);

    public async Task<Credit?> GetByCreditNumberAsync(string creditNumber, CancellationToken ct = default)
        => await context.Credits
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.CreditNumber == creditNumber, ct);

    public async Task<bool> ExistsByCreditNumberAsync(string creditNumber, CancellationToken ct = default)
        => await context.Credits
            .AnyAsync(c => c.CreditNumber == creditNumber, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}
