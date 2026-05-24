using Credits.Api.Controllers;
using Credits.Application.Abstractions;
using Credits.Application.Commands;
using Credits.Application.DTOs.Requests;
using Credits.Application.DTOs.Responses;
using Credits.Application.Tests.FakeData;
using Credits.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Credits.Application.Tests.Controllers;

public sealed class CreditsControllerTests
{
    private readonly Mock<IIntegrateCreditService> _integrateMock = new();
    private readonly Mock<IQueryCreditService> _queryMock = new();
    private readonly CreditsController _controller;

    private static readonly IntegrateCreditRequest ValidRequest = IntegrateCreditFakeData.CreateIntegrateCreditRequest();

    private static CreditResponse MakeResponse(string creditNumber = "123456", string nfseNumber = "789456") =>
        new(creditNumber, nfseNumber, new DateOnly(2024, 2, 25), 1500.75m, "ISSQN", "Sim", 5.0m, 30000m, 5000m, 25000m);

    public CreditsControllerTests()
    {
        _controller = new CreditsController(_integrateMock.Object, _queryMock.Object);
    }

    [Fact]
    public async Task Integrate_Should_Return_202_When_Valid_Request()
    {
        _integrateMock
            .Setup(s => s.IntegrateAsync(It.IsAny<IntegrateCreditsCommand>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _controller.Integrate([ValidRequest], CancellationToken.None);

        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status202Accepted, objectResult.StatusCode);
    }

    [Fact]
    public async Task Integrate_Should_Propagate_BusinessException()
    {
        _integrateMock
            .Setup(s => s.IntegrateAsync(It.IsAny<IntegrateCreditsCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BusinessException("Créditos duplicados."));

        await Assert.ThrowsAsync<BusinessException>(() =>
            _controller.Integrate([ValidRequest], CancellationToken.None));

    }


    [Fact]
    public async Task GetByNfse_Should_Return_200_With_Results()
    {
        _queryMock
            .Setup(s => s.GetByNfseNumberAsync("789456", It.IsAny<CancellationToken>()))
            .ReturnsAsync([MakeResponse()]);

        var result = await _controller.GetByNfse("789456", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
    }

    [Fact]
    public async Task GetByNfse_Should_Return_404_When_No_Credits_Found()
    {
        _queryMock
            .Setup(s => s.GetByNfseNumberAsync("NOTEXIST", It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var result = await _controller.GetByNfse("NOTEXIST", CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, ((NotFoundResult)result).StatusCode);
    }

    [Fact]
    public async Task GetByCredit_Should_Return_200_When_Found()
    {
        _queryMock
            .Setup(s => s.GetByCreditNumberAsync("123456", It.IsAny<CancellationToken>()))
            .ReturnsAsync(MakeResponse());

        var result = await _controller.GetByCredit("123456", CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, ok.StatusCode);
    }

    [Fact]
    public async Task GetByCredit_Should_Return_404_When_Not_Found()
    {
        _queryMock
            .Setup(s => s.GetByCreditNumberAsync("NOTEXIST", It.IsAny<CancellationToken>()))
            .ReturnsAsync((CreditResponse?)null);

        var result = await _controller.GetByCredit("NOTEXIST", CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
        Assert.Equal(StatusCodes.Status404NotFound, ((NotFoundResult)result).StatusCode);
    }
}