namespace Credits.Application.Commands;

public sealed record IngestCreditCommand(
    string CreditNumber,
    string NfseNumber,
    DateOnly ConstitutionDate,
    decimal IssqnAmount,
    string CreditType,
    bool IsSimpleNational,
    decimal TaxRate,
    decimal BilledAmount,
    decimal DeductionAmount,
    decimal CalculationBase);
