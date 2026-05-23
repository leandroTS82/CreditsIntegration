using Credits.Application.Commands;
using Credits.Application.Interfaces;
using Credits.Application.Validators;
using FluentValidation;

namespace Credits.Application.Services;

public class IntegrateCreditService(IValidator<IntegrateCreditsCommand> validator) : IIntegrateCreditService
{
    public async Task IntegrateAsync(IntegrateCreditsCommand command, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(command, ct);
        IntegrateCreditServiceValidator.ValidateBusinessRules(command);
    }
    
}
