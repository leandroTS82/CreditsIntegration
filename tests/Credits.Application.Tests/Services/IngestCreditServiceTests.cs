using Credits.Application.Commands;
using Credits.Application.Services;
using Credits.Application.Tests.FakeData;
using Credits.Domain.Entities;
using Credits.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Credits.Application.Tests.Services;

public sealed class IngestCreditServiceTests
{
    private readonly Mock<ICreditRepository> _repositoryMock = new();
    private readonly IngestCreditService _service;

    private readonly IngestCreditCommand ValidCommand = IngestCreditServiceFakeData.CreateIngestCreditCommand();

    public IngestCreditServiceTests()
    {
        _service = new IngestCreditService(
            _repositoryMock.Object,
            NullLogger<IngestCreditService>.Instance);
    }

    [Fact]
    public async Task Should_Skip_When_Credit_Already_Exists()
    {
        // indepotence test: ingesting the same credit twice should not create duplicates in the database
        _repositoryMock
            .Setup(r => r.ExistsByCreditNumberAsync(ValidCommand.CreditNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _service.IngestAsync(ValidCommand, CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Credit>(), It.IsAny<CancellationToken>()), Times.Never);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Should_Add_And_Save_When_Credit_Does_Not_Exist()
    {
        _repositoryMock
            .Setup(r => r.ExistsByCreditNumberAsync(ValidCommand.CreditNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        await _service.IngestAsync(ValidCommand, CancellationToken.None);

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Credit>(), It.IsAny<CancellationToken>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}