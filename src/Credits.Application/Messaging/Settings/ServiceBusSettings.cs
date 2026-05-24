using System.ComponentModel.DataAnnotations;

namespace Credits.Application.Messaging.Settings;

public sealed class ServiceBusSettings
{
    public const string SectionName = "ServiceBus";

    [Required]
    public string ConnectionString { get; init; } = string.Empty;

    [Required]
    public string Subscription { get; init; } = string.Empty;

    public ServiceBusTopicsSettings Topics { get; init; } = new();
    public ServiceBusConsumerSettings Consumer { get; init; } = new();
}

public sealed class ServiceBusTopicsSettings
{
    [Required]
    public string IntegrateCreditConstituted { get; init; } = string.Empty;
}

public sealed class ServiceBusConsumerSettings
{
    [Range(100, 60_000)]
    public int PollingIntervalMs { get; init; } = 500;

    [Range(50, 5_000)]
    public int MaxWaitTimeMs { get; init; } = 100;
}
