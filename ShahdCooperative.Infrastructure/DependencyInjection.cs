using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShahdCooperative.Domain.Interfaces;
using ShahdCooperative.Domain.Interfaces.Repositories;
using ShahdCooperative.Domain.Services;
using ShahdCooperative.Infrastructure.MessageBroker;
using ShahdCooperative.Infrastructure.MessageBroker.Events;
using ShahdCooperative.Infrastructure.MessageBroker.Handlers;
using ShahdCooperative.Infrastructure.Persistence;
using ShahdCooperative.Infrastructure.Persistence.Repositories;

namespace ShahdCooperative.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext for entity tracking and change detection
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Add Dapper context for high-performance queries
        services.AddSingleton<DapperContext>();

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Domain Services
        services.AddScoped<IInventoryDomainService, InventoryDomainService>();
        services.AddScoped<IOrderDomainService, OrderDomainService>();
        services.AddScoped<IPricingDomainService, PricingDomainService>();

        // Register RabbitMQ Event Handlers
        services.AddScoped<IEventHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();
        services.AddScoped<IEventHandler<UserLoggedInEvent>, UserLoggedInEventHandler>();
        services.AddScoped<IEventHandler<UserLoggedOutEvent>, UserLoggedOutEventHandler>();

        // Register RabbitMQ Consumer as Hosted Service
        services.AddHostedService<RabbitMQConsumer>();

        // Register RabbitMQ Publisher as Singleton
        services.AddSingleton<IEventPublisher, RabbitMQPublisher>();

        return services;
    }
}
