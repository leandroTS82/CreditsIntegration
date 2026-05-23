using Credits.Application.Commands;
using Credits.Application.Interfaces;
using FluentValidation;

namespace Credits.Application.Services;

public class IntegrateCreditService(IValidator<IntegrateCreditsCommand> validator) : IIntegrateCreditService
{
    // configurar comunicação com tópico de msg Azure Service Bus
    public async Task IntegrateAsync(IntegrateCreditsCommand command, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(command, ct);
        
        // chamará SB para enviar a mensagem
    }
}
