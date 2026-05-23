using Credits.Application.Commands;
using Credits.Application.DTOs.Requests;

namespace Credits.Application.Tests.FakeData;

public class IntegrateCreditFakeData
{
    internal static IntegrateCreditRequest CreateIntegrateCreditRequest()
    {
        return new IntegrateCreditRequest
        {
            CreditNumber = "123456",
            NfseNumber = "789456",
            ConstitutionDate = new DateOnly(2026, 5, 22),
            IssqnAmount = 1500.75m,
            CreditType = "ISSQN",
            IsSimpleNational = true,
            TaxRate = 5.0m,
            BilledAmount = 30015.00m,
            DeductionAmount = 0.00m,
            CalculationBase = 30015.00m
        };
    }
    public static IntegrateCreditsCommand CommandWith(Func<IntegrateCreditRequest, IntegrateCreditRequest> modify)
        => new([modify(CreateIntegrateCreditRequest())]);
    public static IEnumerable<object[]> InvalidCommands =>
    [
        [new IntegrateCreditsCommand([])],
        [CommandWith(x => x with { CreditNumber = "" })],
        [CommandWith(x => x with { CreditNumber = new string('A', 51) })],
        [CommandWith(x => x with { NfseNumber = "" })],
        [CommandWith(x => x with { NfseNumber = new string('A', 51) })],
        [CommandWith(x => x with { ConstitutionDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)) })],
        [CommandWith(x => x with { IssqnAmount = 0 })],
        [CommandWith(x => x with { IssqnAmount = -1 })],
        [CommandWith(x => x with { CreditType = "" })],
        [CommandWith(x => x with { CreditType = new string('A', 51) })],
        [CommandWith(x => x with { TaxRate = -1 })],
        [CommandWith(x => x with { BilledAmount = 0 })],
        [CommandWith(x => x with { BilledAmount = -1 })],
        [CommandWith(x => x with { DeductionAmount = -1 })],
        [CommandWith(x => x with { CalculationBase = 0 })],
        [CommandWith(x => x with { CalculationBase = -1 })],
    ];
}
