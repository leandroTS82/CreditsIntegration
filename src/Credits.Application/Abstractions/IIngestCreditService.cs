using Credits.Application.Commands;

namespace Credits.Application.Abstractions;
public interface IIngestCreditService
{
    Task IngestAsync(IngestCreditCommand command, CancellationToken ct = default);
}
