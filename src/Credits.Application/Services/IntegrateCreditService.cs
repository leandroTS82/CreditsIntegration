using Credits.Application.Abstractions;
using Credits.Application.Commands;
using Credits.Application.Messaging.Abstractions;
using Credits.Application.Messaging.Messages;
using Credits.Application.Messaging.Settings;
using Credits.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Credits.Application.Services;

public class IntegrateCreditService(IValidator<IntegrateCreditsCommand> validator, 
    IServiceBusPublisher publisher, 
    IOptions<ServiceBusSettings> settings,
    ILogger<IntegrateCreditService> logger) : IIntegrateCreditService
{
    private readonly ServiceBusSettings _settings = settings.Value;
    public async Task IntegrateAsync(IntegrateCreditsCommand command, CancellationToken ct)
    {
        await validator.ValidateAndThrowAsync(command, ct);
        IntegrateCreditServiceValidator.ValidateBusinessRules(command);

        foreach (var credit in command.Credits)
        {
            var message = new IntegrateCreditMessage(
                credit.CreditNumber,
                credit.NfseNumber,
                credit.ConstitutionDate,
                credit.IssqnAmount,
                credit.CreditType,
                credit.IsSimpleNational,
                credit.TaxRate,
                credit.BilledAmount,
                credit.DeductionAmount,
                credit.CalculationBase
            );

            await publisher.PublishAsync(_settings.Topics.IntegrateCreditConstituted, message, ct);

            logger.LogInformation(
                "Message published for credit {CreditNumber} to topic {Topic}",
                credit.CreditNumber,
                _settings.Topics.IntegrateCreditConstituted);
        }
    }
    
}
