using Credits.Api.IntegrationTests.Fixtures;
using Credits.Api.IntegrationTests.Helpers;
using Credits.Application.Commands;
using Credits.Application.Services;
using Credits.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Credits.Api.IntegrationTests.Tests;

public sealed class IngestCreditIntegrationTests(ApiWebApplicationFactory factory)
    : IClassFixture<ApiWebApplicationFactory>
{
    private IngestCreditService CreateService(IServiceScope scope)
    {
        var repo = scope.ServiceProvider.GetRequiredService<ICreditRepository>();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILogger<IngestCreditService>>();
        return new IngestCreditService(repo, logger);
    }

    private static IngestCreditCommand BuildCommand(string creditNumber,
        string nfseNumber = TestConstants.NfseNumbers.DefaultNumber) =>
            new(creditNumber, nfseNumber, new DateOnly(2024, 1, 15),
                800.50m, "ISSQN", true, 3.5m, 20000m, 3000m, 17000m);

    [Fact]
    public async Task IngestAsync_NewCredit_PersistsToDatabase()
    {
        using var scope = factory.Services.CreateScope();
        var service = CreateService(scope);
        var repo = scope.ServiceProvider.GetRequiredService<ICreditRepository>();

        var command = CreditRequestBuilder.ValidIngestCommand(TestConstants.CreditNumbers.IngestNew);
        await service.IngestAsync(command);

        var exists = await repo.ExistsByCreditNumberAsync(TestConstants.CreditNumbers.IngestNew);
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task IngestAsync_DuplicateCredit_DoesNotInsertAgain()
    {
        // indepotence test: ingesting the same credit twice should not create duplicates in the database
        using var scope = factory.Services.CreateScope();
        var service = CreateService(scope);
        var repo = scope.ServiceProvider.GetRequiredService<ICreditRepository>();

        var command = BuildCommand(TestConstants.CreditNumbers.Duplicate);

        await service.IngestAsync(command);
        await service.IngestAsync(command);

        var credits = await repo.GetByNfseNumberAsync(TestConstants.NfseNumbers.DefaultNumber);
        credits.Count(c => c.CreditNumber == TestConstants.CreditNumbers.Duplicate).Should().Be(1);
    }

    [Fact]
    public async Task IngestAsync_MultipleDistinctCredits_PersistsAll()
    {
        using var scope = factory.Services.CreateScope();
        var service = CreateService(scope);
        var repo = scope.ServiceProvider.GetRequiredService<ICreditRepository>();

        await service.IngestAsync(CreditRequestBuilder.ValidIngestCommand(TestConstants.CreditNumbers.IngestMultipleA,
            TestConstants.NfseNumbers.IngestMultiple));

        await service.IngestAsync(CreditRequestBuilder.ValidIngestCommand(TestConstants.CreditNumbers.IngestMultipleB,
            TestConstants.NfseNumbers.IngestMultiple));

        await service.IngestAsync(CreditRequestBuilder.ValidIngestCommand(TestConstants.CreditNumbers.IngestMultipleC,
            TestConstants.NfseNumbers.IngestMultiple));

        var credits = await repo.GetByNfseNumberAsync(TestConstants.NfseNumbers.IngestMultiple);

        credits.Should().HaveCount(3);
    }
}