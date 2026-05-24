using Credits.Application.DTOs.Responses;
using Credits.Application.Abstractions;
using Credits.Domain.Repositories;

namespace Credits.Application.Services;

public sealed class QueryCreditService(ICreditRepository repository) : IQueryCreditService
{
    public async Task<IReadOnlyList<CreditResponse>> GetByNfseNumberAsync(
        string nfseNumber, CancellationToken ct = default)
    {
        var credits = await repository.GetByNfseNumberAsync(nfseNumber, ct);
        return credits.Select(CreditResponse.FromEntity).ToList();
    }

    public async Task<CreditResponse?> GetByCreditNumberAsync(
        string creditNumber, CancellationToken ct = default)
    {
        var credit = await repository.GetByCreditNumberAsync(creditNumber, ct);
        return credit is null ? null : CreditResponse.FromEntity(credit);
    }
}
