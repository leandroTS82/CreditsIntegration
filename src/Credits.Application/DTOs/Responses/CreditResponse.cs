using Credits.Domain.Entities;

namespace Credits.Application.DTOs.Responses;
public sealed record CreditResponse(
    string CreditNumber,
    string NfseNumber,
    DateOnly ConstitutionDate,
    decimal IssqnAmount,
    string CreditType,
    string SimpleNational,
    decimal TaxRate,
    decimal BilledAmount,
    decimal DeductionAmount,
    decimal CalculationBase)
{
    public static CreditResponse FromEntity(Credit credit) => new(
        credit.CreditNumber,
        credit.NfseNumber,
        credit.ConstitutionDate,
        credit.IssqnAmount,
        credit.CreditType,
        credit.IsSimpleNational ? "Sim" : "Não",
        credit.TaxRate,
        credit.BilledAmount,
        credit.DeductionAmount,
        credit.CalculationBase);
}