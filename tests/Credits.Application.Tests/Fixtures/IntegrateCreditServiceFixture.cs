using Credits.Application.Messaging.Abstractions;
using Credits.Application.Messaging.Settings;
using Credits.Application.Services;
using Credits.Application.Validators;
using Microsoft.Extensions.Options;
using Moq;

namespace Credits.Application.Tests.Fixtures;

public sealed class IntegrateCreditServiceFixture
{
    public Mock<IServiceBusPublisher> PublisherMock { get; } = new();

    public static readonly ServiceBusSettings Settings = new()
    {
        ConnectionString = "fake-connection-string",
        Topics = new ServiceBusTopicsSettings
        {
            IntegrateCreditConstituted = "integrar-credito-constituido-entry"
        }
    };

    public IntegrateCreditService CreateService() => new(
        new IntegrateCreditCommandValidator(),
        PublisherMock.Object,
        Options.Create(Settings));
}
