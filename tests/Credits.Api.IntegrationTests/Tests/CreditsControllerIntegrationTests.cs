using Credits.Api.IntegrationTests.Fixtures;
using Credits.Api.IntegrationTests.Helpers;
using Credits.Domain.Entities;
using Credits.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace Credits.Api.IntegrationTests.Tests;

[Collection("Integration")]
public sealed class CreditsControllerIntegrationTests(ApiWebApplicationFactory factory)
    : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task POST_Integrate_ValidCredits_Returns202WithSuccessTrue()
    {
        var body = new[] { CreditRequestBuilder.ValidCredit(TestConstants.CreditNumbers.Integration, 
            TestConstants.NfseNumbers.Integration) };

        var response = await _client.PostAsJsonAsync(
            "/api/v1/creditos/integrar-credito-constituido", body);

        response.StatusCode.Should().Be(HttpStatusCode.Accepted);

        var content = await response.Content.ReadFromJsonAsync<Dictionary<string, bool>>();
        content!["success"].Should().BeTrue();
    }

    [Fact]
    public async Task POST_Integrate_ValidCredits_PublishesOneMessagePerCredit()
    {
        factory.ServiceBusPublisherMock.Reset();

        var body = new[]
        {
            CreditRequestBuilder.ValidCredit(TestConstants.CreditNumbers.PublicationA, 
            TestConstants.NfseNumbers.Publication),

            CreditRequestBuilder.ValidCredit(TestConstants.CreditNumbers.PublicationB, 
            TestConstants.NfseNumbers.Publication)
        };

        await _client.PostAsJsonAsync(
            "/api/v1/creditos/integrar-credito-constituido", body);

        factory.ServiceBusPublisherMock.Verify(
            p => p.PublishAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }

    [Fact]
    public async Task POST_Integrate_EmptyList_Returns400()
    {
        var body = Array.Empty<object>();

        var response = await _client.PostAsJsonAsync(
            "/api/v1/creditos/integrar-credito-constituido", body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task POST_Integrate_DuplicateCreditNumbers_Returns422()
    {
        var duplicateCredit = CreditRequestBuilder.ValidCredit(TestConstants.CreditNumbers.Duplicate, 
            TestConstants.NfseNumbers.Duplicate);

        var body = new[] { duplicateCredit, duplicateCredit };

        var response = await _client.PostAsJsonAsync(
            "/api/v1/creditos/integrar-credito-constituido", body);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task POST_Integrate_FutureDate_Returns400()
    {
        var body = new[]
        {
            new
            {
                numeroCredito    = TestConstants.CreditNumbers.FutureDate,
                numeroNfse       = TestConstants.NfseNumbers.FutureDate,
                dataConstituicao = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd"),
                valorIssqn       = 1500.75,
                tipoCredito      = "ISSQN",
                simplesNacional  = "Sim",
                aliquota         = 5.0,
                valorFaturado    = 30000.00,
                valorDeducao     = 5000.00,
                baseCalculo      = 25000.00
            }
        };

        var response = await _client.PostAsJsonAsync(
            "/api/v1/creditos/integrar-credito-constituido", body);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GET_ByNfse_ExistingCredits_Returns200WithList()
    {
        await InsertCreditDirectly(TestConstants.CreditNumbers.SearchByNfse, 
            TestConstants.NfseNumbers.SearchByNfse);

        var response = await _client.GetAsync($"/api/v1/creditos/{TestConstants.NfseNumbers.SearchByNfse}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var credits = await response.Content.ReadFromJsonAsync<List<Dictionary<string, object>>>();
        credits.Should().NotBeNullOrEmpty();
        credits!.Should().ContainSingle(c => c["creditNumber"].ToString() == TestConstants.CreditNumbers.SearchByNfse);
    }

    [Fact]
    public async Task GET_ByNfse_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/v1/creditos/{TestConstants.NfseNumbers.NotFound}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_ByCredit_ExistingCredit_Returns200WithDetails()
    {
        await InsertCreditDirectly(TestConstants.CreditNumbers.SearchByNumber, 
            TestConstants.NfseNumbers.SearchByNumber);

        var response = await _client.GetAsync($"/api/v1/creditos/credito/{TestConstants.CreditNumbers.SearchByNumber}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var credit = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        credit.Should().NotBeNull();
        credit!["creditNumber"].ToString().Should().Be(TestConstants.CreditNumbers.SearchByNumber);
    }

    [Fact]
    public async Task GET_ByCredit_NotFound_Returns404()
    {
        var response = await _client.GetAsync($"/api/v1/creditos/credito/{TestConstants.CreditNumbers.NotFound}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    private async Task InsertCreditDirectly(string creditNumber, string nfseNumber)
    {
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var credit = Credit.Create(
            creditNumber,
            nfseNumber,
            new DateOnly(2024, 2, 25),
            issqnAmount: 1500.75m,
            creditType: "ISSQN",
            isSimpleNational: true,
            taxRate: 5.0m,
            billedAmount: 30000m,
            deductionAmount: 5000m,
            calculationBase: 25000m);

        await db.Set<Credit>().AddAsync(credit);
        await db.SaveChangesAsync();
    }
}