using Credits.Application.Commands;
using Credits.Application.Tests.FakeData;
using Credits.Application.Tests.Fixtures;
using Credits.Domain.Exceptions;
using FluentValidation;

namespace Credits.Application.Tests.Services;

public sealed class IntegrateCreditServiceTests
{
    private readonly IntegrateCreditServiceFixture _fixture = new();

    [Theory]
    [MemberData(nameof(IntegrateCreditFakeData.InvalidCommands), MemberType = typeof(IntegrateCreditFakeData))]
    public async Task Should_Throw_ValidationException_When_InvalidCommand_IntegrateAsync(IntegrateCreditsCommand command)
    {
        var service = _fixture.CreateService();
        await Assert.ThrowsAsync<ValidationException>(() =>
            service.IntegrateAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task Should_Throw_BusinessException_When_DuplicateCreditNumbers_IntegrateAsync()
    {
        var service = _fixture.CreateService();
        var command = IntegrateCreditFakeData.CreateIntegrateCreditDuplicatedRequest();

        await Assert.ThrowsAsync<BusinessException>(() =>
            service.IntegrateAsync(command, CancellationToken.None));
    }
}