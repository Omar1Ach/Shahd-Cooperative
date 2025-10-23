using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShahdCooperative.Domain.Interfaces.Repositories;
using ShahdCooperative.Domain.Interfaces.Services;
using ShahdCooperative.Infrastructure.ExternalServices;
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

        // Register external services with HttpClient
        services.AddHttpClient<IAuthService, AuthService>();
        services.AddHttpClient<INotificationService, NotificationService>();

        return services;
    }
}
