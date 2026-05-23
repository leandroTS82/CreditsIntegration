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
            });
    }
}