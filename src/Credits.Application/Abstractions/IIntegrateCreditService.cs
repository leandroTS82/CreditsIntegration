using Credits.Application.Commands;

namespace Credits.Application.Abstractions;

public interface IIntegrateCreditService
{
    Task IntegrateAsync(IntegrateCreditsCommand command, CancellationToken ct);
}
