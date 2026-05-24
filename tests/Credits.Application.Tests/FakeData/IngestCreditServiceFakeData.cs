using Credits.Application.Commands;

namespace Credits.Application.Tests.FakeData;

public class IngestCreditServiceFakeData
{
    internal static IngestCreditCommand CreateIngestCreditCommand()
    {
        var fakeCommand = new IngestCreditCommand(
           CreditNumber: "123456",
           NfseNumber: "789456",
           ConstitutionDate: new DateOnly(2024, 2, 25),
           IssqnAmount: 1500.75m,
           CreditType: "ISSQN",
           IsSimpleNational: true,
           TaxRate: 5.0m,
           BilledAmount: 30000.00m,
           DeductionAmount: 5000.00m,
           CalculationBase: 25000.00m);

       return fakeCommand;
    }
}
