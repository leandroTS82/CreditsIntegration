using System.Text.Json.Serialization;

namespace Credits.Application.DTOs.Requests;

public sealed record IntegrateCreditRequest
{
    [JsonPropertyName("numeroCredito")]
    public required string CreditNumber { get; init; }

    [JsonPropertyName("numeroNfse")]
    public required string NfseNumber { get; init; }

    [JsonPropertyName("dataConstituicao")]
    public required DateOnly ConstitutionDate { get; init; }

    [JsonPropertyName("valorIssqn")]
    public required decimal IssqnAmount { get; init; }

    [JsonPropertyName("tipoCredito")]
    public required string CreditType { get; init; }

    [JsonPropertyName("simplesNacional")]
    public required bool IsSimpleNational { get; init; }

    [JsonPropertyName("aliquota")]
    public required decimal TaxRate { get; init; }

    [JsonPropertyName("valorFaturado")]
    public required decimal BilledAmount { get; init; }

    [JsonPropertyName("valorDeducao")]
    public required decimal DeductionAmount { get; init; }

    [JsonPropertyName("baseCalculo")]
    public required decimal CalculationBase { get; init; }
}