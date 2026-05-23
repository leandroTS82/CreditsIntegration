using Credits.Application.Interfaces;
using Credits.Application.Services;
using Credits.Application.Validators;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace Credits.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IIntegrateCreditService, IntegrateCreditService>();

        services.AddValidatorsFromAssemblyContaining<IntegrateCreditCommandValidator>();
        return services;
    }
}
