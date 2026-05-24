using Credits.Application.Abstractions;
using Credits.Application.DTOs.Responses;
using Credits.Domain.Messaging.Abstractions;
using Credits.Domain.Messaging.Notifications;
using Credits.Domain.Repositories;

namespace Credits.Application.Services;

public sealed class QueryCreditService(ICreditRepository repository, INotificationPublisher notifier) : IQueryCreditService
{
    public async Task<IReadOnlyList<CreditResponse>> GetByNfseNumberAsync(
        string nfseNumber, CancellationToken ct = default)
    {
        var credits = await repository.GetByNfseNumberAsync(nfseNumber, ct);

        await notifier.NotifyAsync(new NotificationAuditMessage(
            EventType: "query.by-nfse",
            Key: nfseNumber,
            OccurredAt: DateTimeOffset.UtcNow), ct);

        return credits.Select(CreditResponse.FromEntity).ToList();
    }

    public async Task<CreditResponse?> GetByCreditNumberAsync(
        string creditNumber, CancellationToken ct = default)
    {
        var credit = await repository.GetByCreditNumberAsync(creditNumber, ct);

        await notifier.NotifyAsync(new NotificationAuditMessage(
            EventType: "query.by-credit",
            Key: creditNumber,
            OccurredAt: DateTimeOffset.UtcNow), ct);

        return credit is null ? null : CreditResponse.FromEntity(credit);
    }
}
