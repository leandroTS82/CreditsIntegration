using Credits.Application.DTOs.Responses;

namespace Credits.Application.Abstractions;
public interface IQueryCreditService
{
    Task<IReadOnlyList<CreditResponse>> GetByNfseNumberAsync(string nfseNumber, CancellationToken ct = default);
    Task<CreditResponse?> GetByCreditNumberAsync(string creditNumber, CancellationToken ct = default);
}