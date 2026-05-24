namespace Credits.Domain.Messaging.Messages;

public sealed record IntegrateCreditMessage(
    string CreditNumber,
    string NfseNumber,
    DateOnly ConstitutionDate,
    decimal IssqnAmount,
    string CreditType,
    bool IsSimpleNational,
    decimal TaxRate,
    decimal BilledAmount,
    decimal DeductionAmount,
    decimal CalculationBase
);
