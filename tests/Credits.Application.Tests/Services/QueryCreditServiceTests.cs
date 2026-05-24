using Credits.Domain.Messaging.Abstractions;
using Credits.Application.Services;
using Credits.Domain.Entities;
using Credits.Domain.Repositories;
using Moq;

namespace Credits.Application.Tests.Services;

public sealed class QueryCreditServiceTests
{
    private readonly Mock<ICreditRepository> _repositoryMock = new();
    private readonly Mock<INotificationPublisher> _notifierMock = new();
    private readonly QueryCreditService _service;
    private readonly string _nfseNumber = "789456";

    public QueryCreditServiceTests()
    {
        _service = new QueryCreditService(_repositoryMock.Object, _notifierMock.Object);
    }

    Credit MakeCredit(string creditNumber = "123456", string nfseNumber = "789456", bool simpleNational = true) =>
        Credit.Create(
            creditNumber,
            nfseNumber,
            new DateOnly(2024, 2, 25),
            1500.75m,
            "ISSQN",
            simpleNational,
            5.0m,
            30000.00m,
            5000.00m,
            25000.00m);


    [Fact]
    public async Task GetByNfseNumber_Should_Return_Mapped_Credits()
    {
        var credits = new List<Credit>
        {
            MakeCredit("111", _nfseNumber),
            MakeCredit("222", _nfseNumber),
            MakeCredit("333", _nfseNumber),
        };

        _repositoryMock
            .Setup(r => r.GetByNfseNumberAsync(_nfseNumber, It.IsAny<CancellationToken>()))
            .ReturnsAsync(credits);

        var result = await _service.GetByNfseNumberAsync(_nfseNumber);

        Assert.Equal(3, result.Count);
        Assert.Equal("111", result[0].CreditNumber);
        Assert.Equal("222", result[1].CreditNumber);
        Assert.Equal("333", result[2].CreditNumber);
    }

    [Fact]
    public async Task GetByNfseNumber_Should_Map_SimpleNational_True_To_Sim()
    {
        _repositoryMock
            .Setup(r => r.GetByNfseNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([MakeCredit(simpleNational: true)]);

        var result = await _service.GetByNfseNumberAsync(_nfseNumber);

        Assert.Equal("Sim", result[0].SimpleNational);
    }

    [Fact]
    public async Task GetByNfseNumber_Should_Map_SimpleNational_False_To_Nao()
    {
        _repositoryMock
            .Setup(r => r.GetByNfseNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([MakeCredit(simpleNational: false)]);

        var result = await _service.GetByNfseNumberAsync(_nfseNumber);

        Assert.Equal("Não", result[0].SimpleNational);
    }

    // get by credit number tests

    [Fact]
    public async Task GetByCreditNumber_Should_Return_Null_When_Not_Found()
    {
        _repositoryMock
            .Setup(r => r.GetByCreditNumberAsync("NOTEXIST", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Credit?)null);

        var result = await _service.GetByCreditNumberAsync("NOTEXIST");

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCreditNumber_Should_Return_Mapped_Credit_When_Found()
    {
        var credit = MakeCredit("123456");
        _repositoryMock
            .Setup(r => r.GetByCreditNumberAsync("123456", It.IsAny<CancellationToken>()))
            .ReturnsAsync(credit);

        var result = await _service.GetByCreditNumberAsync("123456");

        Assert.NotNull(result);
        Assert.Equal("123456", result.CreditNumber);
    }
}