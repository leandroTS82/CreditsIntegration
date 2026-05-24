using Credits.Application.Abstractions;
using Credits.Application.Commands;
using Credits.Domain.Entities;
using Credits.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace Credits.Application.Services;

public sealed class IngestCreditService(
    ICreditRepository repository,
    ILogger<IngestCreditService> logger) : IIngestCreditService
{
    public async Task IngestAsync(IngestCreditCommand command, CancellationToken ct = default)
    {
        if (await repository.ExistsByCreditNumberAsync(command.CreditNumber, ct))
        {
            logger.LogWarning("Credit {CreditNumber} already exists. Skipping.", command.CreditNumber);
            return;
        }

        var credit = Credit.Create(
            command.CreditNumber,
            command.NfseNumber,
            command.ConstitutionDate,
            command.IssqnAmount,
            command.CreditType,
            command.IsSimpleNational,
            command.TaxRate,
            command.BilledAmount,
            command.DeductionAmount,
            command.CalculationBase);

        await repository.AddAsync(credit, ct);
        await repository.SaveChangesAsync(ct);

        logger.LogInformation("Credit {CreditNumber} ingested successfully.", command.CreditNumber);
    }
}