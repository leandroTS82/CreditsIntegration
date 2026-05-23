using Credits.Application.Commands;
using Credits.Application.DTOs.Requests;
using Credits.Application.Services;
using Credits.Application.Tests.FakeData;
using Credits.Application.Validators;
using FluentValidation;

namespace Credits.Application.Tests.Services;

public sealed class IntegrateCreditServiceTests
{
    private readonly IntegrateCreditService _service = new(new IntegrateCreditCommandValidator());

    [Theory]
    [MemberData(nameof(IntegrateCreditFakeData.InvalidCommands), MemberType = typeof(IntegrateCreditFakeData))]
    public async Task Should_Throw_ValidationException_When_InvalidCommand_IntegrateAsync(IntegrateCreditsCommand command)
    {
        await Assert.ThrowsAsync<ValidationException>(() =>
            _service.IntegrateAsync(command, CancellationToken.None));
    }
}