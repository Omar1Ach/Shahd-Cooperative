using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ShahdCooperative.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        services.AddAutoMapper(assembly);
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
