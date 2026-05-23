using Credits.Application.Commands;
using Credits.Application.DTOs.Requests;
using Credits.Application.Tests.FakeData;
using Credits.Application.Validators;
using FluentValidation.TestHelper;

namespace Credits.Application.Tests.Validators;

public sealed class IntegrateCreditCommandValidatorTests
{
    private readonly IntegrateCreditCommandValidator _validator = new();
    [Fact]
    public void Should_Not_Have_Errors_When_ValidCommand_Validate()
    {
        var command = new IntegrateCreditsCommand([IntegrateCreditFakeData.CreateIntegrateCreditRequest()]);

        var result = _validator.TestValidate(command);

        result.ShouldNotHaveAnyValidationErrors();
    }
    [Theory]
    [MemberData(nameof(IntegrateCreditFakeData.InvalidCommands), MemberType = typeof(IntegrateCreditFakeData))]
    public void Should_Have_Errors_When_InvalidCommand_Validate(IntegrateCreditsCommand command)
    {
        var result = _validator.TestValidate(command);
        result.ShouldHaveAnyValidationError();
    }
}
