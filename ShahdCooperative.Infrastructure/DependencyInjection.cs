using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShahdCooperative.Domain.Interfaces.Repositories;
using ShahdCooperative.Infrastructure.Persistence;
using ShahdCooperative.Infrastructure.Persistence.Repositories;

namespace ShahdCooperative.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Add Dapper context for database access
        services.AddSingleton<DapperContext>();

        // Register repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
