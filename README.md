# ShahdCooperative - Beekeeping Cooperative Backend System

## 📋 Project Overview

Complete .NET 8 backend system following **Clean Architecture** principles for a beekeeping cooperative that manages:
- **Bee Products**: honey, wax, pollen, propolis, royal jelly
- **Beekeeping Equipment**: hives, protective suits, tools, feeders, extractors

### Architecture

The system follows the **Clean Architecture** pattern with four distinct layers:

```
┌─────────────────────────────────────────┐
│   Presentation Layer (API)              │  ← REST Controllers, Middleware
├─────────────────────────────────────────┤
│   Application Layer                     │  ← Use Cases, DTOs, MediatR
├─────────────────────────────────────────┤
│   Domain Layer (Core)                   │  ← Entities, Interfaces
├─────────────────────────────────────────┤
│   Infrastructure Layer                  │  ← EF Core, External Services
└─────────────────────────────────────────┘
```

---

## ✅ Current Status

### **✓ COMPLETED**

#### 1. Solution Structure
- ✅ Solution file (`ShahdCooperative.sln`)
- ✅ 4 main projects (Domain, Application, Infrastructure, API)
- ✅ 4 test projects (all layers)
- ✅ All project references configured correctly
- ✅ **Solution builds successfully without errors**

#### 2. NuGet Packages Installed
- ✅ **Application**: MediatR 13.0, FluentValidation 12.0, AutoMapper 15.0
- ✅ **Infrastructure**: EF Core 9.0, SQL Server 9.0, RabbitMQ.Client 7.1
- ✅ **API**: Swagger 9.0, JWT Bearer 8.0, Serilog 9.0
- ✅ **Tests**: xUnit, FluentAssertions, Moq, AutoFixture

#### 3. Domain Layer (100% Complete)
- ✅ **Base Entity**: Common properties (Id, CreatedAt, UpdatedAt, IsDeleted)
- ✅ **8 Entities**: Product, Order, OrderItem, Customer, Admin, NotificationLog, Feedback
- ✅ **4 Enums**: ProductType, OrderStatus, NotificationType, NotificationStatus
- ✅ **Repository Interfaces**:
  - IRepository<T> (generic)
  - IProductRepository, IOrderRepository, ICustomerRepository, IFeedbackRepository
  - IUnitOfWork
- ✅ **Service Interfaces**: IAuthService, INotificationService
- ✅ **Domain Exceptions**: DomainException base class

#### 4. Folder Structure
- ✅ All layers have complete folder hierarchy created
- ✅ Features organized by domain (Products, Orders, Inventory, Customers, Feedback, Reports)
- ✅ Separate folders for Commands and Queries (CQRS pattern)

---

## 📦 Solution Projects

### **ShahdCooperative.Domain** (Class Library - .NET 8)
**Status**: ✅ Complete
**Dependencies**: None (pure domain logic)
**Contents**:
- `Entities/` - 8 domain entities with navigation properties
- `Enums/` - 4 enumerations
- `Interfaces/Repositories/` - Repository interfaces
- `Interfaces/Services/` - External service interfaces
- `Exceptions/` - Domain exceptions

### **ShahdCooperative.Application** (Class Library - .NET 8)
**Status**: 🔨 Structure ready, implementation needed
**Dependencies**: Domain only
**Packages**: MediatR, FluentValidation, AutoMapper
**Folders Created**:
- `DTOs/` - Request/Response objects (Products, Orders, Customers, Feedback, Reports)
- `Features/` - CQRS structure:
  - `Products/Commands/` - CreateProduct, UpdateProduct, DeleteProduct
  - `Products/Queries/` - GetProducts, GetProductById
  - `Orders/Commands/` - CreateOrder, UpdateOrderStatus, CancelOrder
  - `Orders/Queries/` - GetOrders, GetOrderById
  - `Inventory/Commands/` - UpdateStock
  - `Inventory/Queries/` - GetLowStockProducts
  - `Customers/Queries/` - GetCustomers
  - `Feedback/Commands/` - CreateFeedback
  - `Feedback/Queries/` - GetFeedback
  - `Reports/Queries/` - GetSalesReport
- `Mappings/` - AutoMapper profiles
- `Validators/` - FluentValidation validators
- `Common/` - Behaviors, Exceptions, Result models

**TODO**: Implement command/query handlers, DTOs, validators, and mappings

### **ShahdCooperative.Infrastructure** (Class Library - .NET 8)
**Status**: 🔨 Structure ready, implementation needed
**Dependencies**: Application, Domain
**Packages**: EF Core 9.0, SQL Server, RabbitMQ.Client
**Folders Created**:
- `Persistence/` - DbContext, Configurations, Repositories
- `ExternalServices/` - AuthServiceAdapter, NotificationServiceAdapter
- `MessageBroker/` - RabbitMQ integration, Events
- `Common/` - DateTimeProvider, GuidProvider

**TODO**: Implement DbContext, entity configurations, repository implementations, UnitOfWork, external adapters

### **ShahdCooperative.API** (ASP.NET Core Web API - .NET 8)
**Status**: 🔨 Structure ready, implementation needed
**Dependencies**: Application, Infrastructure
**Packages**: Swagger, JWT Bearer, Serilog
**Folders Created**:
- `Controllers/` - REST API endpoints
- `Middleware/` - ExceptionHandling, RequestLogging, JwtValidation
- `Filters/` - ValidationFilter, AuthorizationFilter
- `Extensions/` - ServiceCollectionExtensions

**TODO**: Implement controllers, middleware, configure Program.cs, setup appsettings.json

### **Test Projects**
**Status**: 🔨 Projects created, tests need implementation
**Packages**: xUnit, FluentAssertions, Moq, AutoFixture

---

## 🚀 Quick Start

### Prerequisites
- .NET 8 SDK
- SQL Server or PostgreSQL
- (Optional) RabbitMQ for message broker
- IDE: Visual Studio 2022, VS Code, or Rider

### Build the Solution

```bash
cd C:\Users\Predator\ShahdCooperative
dotnet build
```

**Result**: ✅ Build succeeds with 0 errors (only warnings about AutoMapper version)

### Run the API (After completing TODOs)

```bash
dotnet run --project ShahdCooperative.API
```

---

## 📝 Next Steps (Implementation TODO)

The foundation is complete and the solution builds successfully. Here's what needs to be implemented:

### Priority 1: Infrastructure Layer (Database)

#### A. ApplicationDbContext
Create `Infrastructure/Persistence/ApplicationDbContext.cs`:
```csharp
- Define DbSets for all entities
- Configure database connection
- Override SaveChangesAsync for audit fields (CreatedAt, UpdatedAt)
```

#### B. Entity Configurations
Create Fluent API configurations in `Infrastructure/Persistence/Configurations/`:
```csharp
- ProductConfiguration.cs
- OrderConfiguration.cs
- OrderItemConfiguration.cs
- CustomerConfiguration.cs
- AdminConfiguration.cs
- NotificationLogConfiguration.cs
- FeedbackConfiguration.cs
```

**Example Template**:
```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Price).HasPrecision(18, 2);
        // Add indexes, relationships, constraints
    }
}
```

#### C. Repository Implementations
Create in `Infrastructure/Persistence/Repositories/`:
```csharp
- Repository<T>.cs (generic implementation)
- ProductRepository.cs
- OrderRepository.cs
- CustomerRepository.cs
- FeedbackRepository.cs
- UnitOfWork.cs
```

#### D. Dependency Injection
Create `Infrastructure/DependencyInjection.cs`:
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register external services
        services.AddScoped<IAuthService, AuthServiceAdapter>();
        services.AddScoped<INotificationService, NotificationServiceAdapter>();

        return services;
    }
}
```

### Priority 2: Application Layer (Use Cases)

#### A. Common Models
Create in `Application/Common/Models/`:
```csharp
- Result.cs
- Result<T>.cs
- PagedResult<T>.cs
- Error.cs
```

#### B. DTOs
Create request/response DTOs in respective folders:
```csharp
DTOs/Products/
- CreateProductRequest.cs
- UpdateProductRequest.cs
- ProductResponse.cs

DTOs/Orders/
- CreateOrderRequest.cs
- OrderItemDto.cs
- OrderResponse.cs

(Continue for all features)
```

#### C. MediatR Commands/Queries
For each feature, create Command/Query, Handler, and Validator:

**Example**: `Features/Products/Commands/CreateProduct/`
```csharp
// CreateProductCommand.cs
public record CreateProductCommand(
    string Name,
    string Description,
    string Category,
    decimal Price,
    int StockQuantity,
    ProductType Type
) : IRequest<Result<ProductResponse>>;

// CreateProductCommandHandler.cs
public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public async Task<Result<ProductResponse>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        // TODO: Implement business logic
    }
}

// CreateProductCommandValidator.cs
public class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Price).GreaterThan(0);
        // Add more validation rules
    }
}
```

#### D. AutoMapper Profiles
Create in `Application/Mappings/`:
```csharp
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductResponse>();
        CreateMap<CreateProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>();
    }
}
```

#### E. MediatR Behaviors
Create in `Application/Common/Behaviors/`:
```csharp
- ValidationBehavior.cs (validates commands using FluentValidation)
- LoggingBehavior.cs (logs all requests)
```

#### F. Dependency Injection
Create `Application/DependencyInjection.cs`:
```csharp
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(assembly));

        services.AddAutoMapper(assembly);

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(LoggingBehavior<,>));

        return services;
    }
}
```

### Priority 3: API Layer (Presentation)

#### A. Controllers
Create REST controllers in `API/Controllers/`:
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<ProductResponse>>> GetProducts()
    {
        var query = new GetProductsQuery();
        var result = await _mediator.Send(query);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResponse>> CreateProduct(
        CreateProductRequest request)
    {
        var command = new CreateProductCommand(...);
        var result = await _mediator.Send(command);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetProductById),
                new { id = result.Value.Id }, result.Value)
            : BadRequest(result.Error);
    }
}
```

**Controllers to Create**:
- ProductsController.cs
- OrdersController.cs
- CustomersController.cs
- FeedbackController.cs
- ReportsController.cs
- InventoryController.cs

#### B. Middleware
Create in `API/Middleware/`:
```csharp
- ExceptionHandlingMiddleware.cs
- RequestLoggingMiddleware.cs
```

#### C. appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ShahdCooperativeDb;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "ExternalServices": {
    "AuthServiceUrl": "https://localhost:5001",
    "NotificationServiceUrl": "https://localhost:5002"
  },
  "MessageBroker": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest"
  },
  "Jwt": {
    "ValidAudience": "https://localhost:7000",
    "ValidIssuer": "https://localhost:5001",
    "Secret": "YOUR_SECRET_KEY_HERE_MINIMUM_32_CHARACTERS"
  },
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "File",
        "Args": {
          "path": "Logs/log-.txt",
          "rollingInterval": "Day"
        }
      }
    ]
  },
  "AllowedHosts": "*",
  "Cors": {
    "AllowedOrigins": [ "http://localhost:3000", "http://localhost:4200" ]
  }
}
```

#### D. Program.cs
```csharp
using Serilog;
using ShahdCooperative.Application;
using ShahdCooperative.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to container
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ShahdCooperative API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:ValidIssuer"],
            ValidAudience = builder.Configuration["Jwt:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

builder.Services.AddAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:AllowedOrigins")
                    .Get<string[]>() ?? Array.Empty<string>())
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Priority 4: Database Setup

#### Initialize Database
```bash
# Add initial migration
dotnet ef migrations add InitialCreate --project ShahdCooperative.Infrastructure --startup-project ShahdCooperative.API

# Update database
dotnet ef database update --project ShahdCooperative.Infrastructure --startup-project ShahdCooperative.API
```

---

## 🎯 Implementation Order

Follow this order for efficient implementation:

1. **Week 1: Infrastructure**
   - [ ] ApplicationDbContext
   - [ ] Entity Configurations
   - [ ] Generic Repository
   - [ ] Specific Repositories
   - [ ] UnitOfWork
   - [ ] DependencyInjection

2. **Week 2: Application - Core Models**
   - [ ] Result<T>, PagedResult<T>
   - [ ] Common DTOs
   - [ ] AutoMapper Profiles
   - [ ] MediatR Behaviors

3. **Week 3: Application - Products Feature**
   - [ ] Product DTOs
   - [ ] CreateProduct Command + Handler + Validator
   - [ ] UpdateProduct Command + Handler + Validator
   - [ ] DeleteProduct Command + Handler
   - [ ] GetProducts Query + Handler
   - [ ] GetProductById Query + Handler

4. **Week 4: Application - Orders Feature**
   - [ ] Order DTOs
   - [ ] CreateOrder Command + Handler + Validator
   - [ ] UpdateOrderStatus Command + Handler
   - [ ] CancelOrder Command + Handler
   - [ ] GetOrders Query + Handler
   - [ ] GetOrderById Query + Handler

5. **Week 5: Application - Remaining Features**
   - [ ] Inventory (UpdateStock, GetLowStockProducts)
   - [ ] Customers (GetCustomers, GetCustomerById)
   - [ ] Feedback (CreateFeedback, GetFeedback)
   - [ ] Reports (GetSalesReport, GetTopCustomers)

6. **Week 6: API Layer**
   - [ ] ProductsController
   - [ ] OrdersController
   - [ ] InventoryController
   - [ ] CustomersController
   - [ ] FeedbackController
   - [ ] ReportsController
   - [ ] Middleware (Exception, Logging)
   - [ ] Program.cs configuration
   - [ ] appsettings.json

7. **Week 7: Testing**
   - [ ] Domain unit tests
   - [ ] Application integration tests
   - [ ] API integration tests
   - [ ] End-to-end tests

---

## 📚 Resources

### Clean Architecture
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [.NET Microservices Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/)

### MediatR & CQRS
- [MediatR Documentation](https://github.com/jbogard/MediatR)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)

### Entity Framework Core
- [EF Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [Fluent API Configuration](https://docs.microsoft.com/en-us/ef/core/modeling/)

---

## 🔧 Troubleshooting

### AutoMapper Version Warning
The warning about AutoMapper version mismatch is harmless and doesn't affect functionality. To fix:
```bash
dotnet add package AutoMapper --version 12.0.1
```

### Database Connection Issues
1. Update connection string in `appsettings.json`
2. Ensure SQL Server is running
3. Check firewall settings

### JWT Authentication Not Working
1. Verify `Jwt:Secret` is at least 32 characters
2. Ensure token is passed in Authorization header: `Bearer {token}`
3. Check token expiration

---

## 📄 License

This project is developed for the Shahd Beekeeping Cooperative.

---

## 👥 Contributors

- Initial Setup: Claude Code Assistant
- Architecture Design: Based on UML diagrams

---

## 📞 Support

For questions or issues:
1. Check this README
2. Review UML diagrams in `beekeeping-uml-diagrams/` folder
3. Consult .NET 8 documentation

---

**Current Version**: 1.0.0-alpha
**Last Updated**: 2025-10-22
**Build Status**: ✅ Builds Successfully
**Test Coverage**: TBD (tests need implementation)

---

## Quick Commands

```bash
# Build solution
dotnet build

# Run API
dotnet run --project ShahdCooperative.API

# Run tests
dotnet test

# Add migration
dotnet ef migrations add MigrationName --project ShahdCooperative.Infrastructure --startup-project ShahdCooperative.API

# Update database
dotnet ef database update --project ShahdCooperative.Infrastructure --startup-project ShahdCooperative.API

# Create new controller
dotnet new apicontroller -n ProductsController -o ShahdCooperative.API/Controllers
```

---

**🎉 Congratulations! The foundation is complete and ready for implementation.**
