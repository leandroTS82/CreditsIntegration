using Credits.Domain.Entities;
using Credits.Application.Commands;

namespace Credits.Api.IntegrationTests.Helpers;

public static class CreditRequestBuilder
{
    public static object ValidCredit(
        string creditNumber = "credito-padrao",
        string nfseNumber = "nfse-padrao") => new
        {
            numeroCredito = creditNumber,
            numeroNfse = nfseNumber,
            dataConstituicao = "2024-02-25",
            valorIssqn = 1500.75,
            tipoCredito = "ISSQN",
            simplesNacional = "Sim",
            aliquota = 5.0,
            valorFaturado = 30000.00,
            valorDeducao = 5000.00,
            baseCalculo = 25000.00
        };

    public static Credit ValidCreditEntity(
        string creditNumber = "credito-padrao",
        string nfseNumber = "nfse-padrao") =>
        Credit.Create(creditNumber, nfseNumber,
            new DateOnly(2024, 2, 25), 1500.75m,
            "ISSQN", true, 5.0m, 30000m, 5000m, 25000m);

    public static IngestCreditCommand ValidIngestCommand(
        string creditNumber = "credito-padrao",
        string nfseNumber = "nfse-padrao") =>
        new(creditNumber, nfseNumber,
            new DateOnly(2024, 1, 15),
            800.50m, "ISSQN", true, 3.5m, 20000m, 3000m, 17000m);
}