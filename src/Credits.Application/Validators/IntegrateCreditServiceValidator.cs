using Credits.Application.Commands;
using Credits.Domain.Exceptions;

namespace Credits.Application.Validators;

public static class IntegrateCreditServiceValidator
{
    public static void ValidateBusinessRules(IntegrateCreditsCommand command)
    {
        var duplicates = command.Credits
            .GroupBy(c => c.CreditNumber)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicates.Count > 0)
            throw new BusinessException(
                $"Créditos duplicados na requisição: {string.Join(", ", duplicates)}.");

        //The test document does not require validation,
        //input, or business logic, so I added only a test to map the case; in this scenario,
        //validations can be extended.
    }
}
