using Credits.Application.Commands;

namespace Credits.Application.Interfaces;

public interface IIntegrateCreditService
{
    Task IntegrateAsync(IntegrateCreditsCommand command, CancellationToken ct);
}
