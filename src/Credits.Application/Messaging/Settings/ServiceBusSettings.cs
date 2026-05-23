using System.ComponentModel.DataAnnotations;

namespace Credits.Application.Messaging.Settings;

public sealed class ServiceBusSettings
{
    public const string SectionName = "ServiceBus";

    [Required]
    public string ConnectionString { get; init; } = string.Empty;
    public ServiceBusTopicsSettings Topics { get; init; } = new();
}

public sealed class ServiceBusTopicsSettings
{
    [Required]
    public string IntegrateCreditConstituted { get; init; } = string.Empty;
}
