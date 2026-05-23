using Credits.Application.Commands;
using FluentValidation;

namespace Credits.Application.Validators;

public sealed class IntegrateCreditCommandValidator
    : AbstractValidator<IntegrateCreditsCommand>
{
    public IntegrateCreditCommandValidator()
    {
        RuleFor(x => x.Credits)
            .NotEmpty();

        RuleForEach(x => x.Credits)
            .ChildRules(credit =>
            {
                credit.RuleFor(x => x.CreditNumber)
                    .NotEmpty()
                    .MaximumLength(50);

                credit.RuleFor(x => x.NfseNumber)
                    .NotEmpty();

                credit.RuleFor(x => x.IssqnAmount)
                    .GreaterThan(0);

                credit.RuleFor(x => x.TaxRate)
                    .GreaterThanOrEqualTo(0);

                credit.RuleFor(x => x.CalculationBase)
                    .GreaterThan(0);

                credit.RuleFor(x => x.ConstitutionDate)
                    .NotEmpty()
                    .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow))
                    .WithMessage("Data de constituição não pode ser futura.");

                credit.RuleFor(x => x.CreditType)
                    .NotEmpty()
                    .MaximumLength(50);

                credit.RuleFor(x => x.NfseNumber)
                    .NotEmpty()
                    .MaximumLength(50);

                credit.RuleFor(x => x.BilledAmount)
                    .GreaterThan(0);

                credit.RuleFor(x => x.DeductionAmount)
                    .GreaterThanOrEqualTo(0);

            });
    }
}