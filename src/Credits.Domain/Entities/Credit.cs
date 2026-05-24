namespace Credits.Domain.Entities;

public sealed class Credit
{
    public long Id { get; private set; }
    public string CreditNumber { get; private set; } = string.Empty;
    public string NfseNumber { get; private set; } = string.Empty;
    public DateOnly ConstitutionDate { get; private set; }
    public decimal IssqnAmount { get; private set; }
    public string CreditType { get; private set; } = string.Empty;
    public bool IsSimpleNational { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal BilledAmount { get; private set; }
    public decimal DeductionAmount { get; private set; }
    public decimal CalculationBase { get; private set; }

    private Credit() { }

    public static Credit Create(
        string creditNumber,
        string nfseNumber,
        DateOnly constitutionDate,
        decimal issqnAmount,
        string creditType,
        bool isSimpleNational,
        decimal taxRate,
        decimal billedAmount,
        decimal deductionAmount,
        decimal calculationBase)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(creditNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(nfseNumber);
        ArgumentException.ThrowIfNullOrWhiteSpace(creditType);

        return new Credit
        {
            CreditNumber = creditNumber,
            NfseNumber = nfseNumber,
            ConstitutionDate = constitutionDate,
            IssqnAmount = issqnAmount,
            CreditType = creditType,
            IsSimpleNational = isSimpleNational,
            TaxRate = taxRate,
            BilledAmount = billedAmount,
            DeductionAmount = deductionAmount,
            CalculationBase = calculationBase
        };
    }
}