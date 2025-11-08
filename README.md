# ShahdCooperative Main Service

**Enterprise-grade e-commerce microservice** for beekeeping cooperative business operations, implementing comprehensive product management, order processing, customer management, and feedback systems.

**Developer:** Omar Achbani - Full-Stack React .NET Developer
**Framework:** ASP.NET Core
**Architecture:** Clean Architecture with Domain-Driven Design (DDD)

---

## Table of Contents

- [Overview](#overview)
- [Business Domain](#business-domain)
- [Architecture](#architecture)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
- [Integration](#integration)
- [Getting Started](#getting-started)
- [Configuration](#configuration)

---

## Overview

ShahdCooperative Main Service is the core business logic microservice for a beekeeping cooperative's e-commerce platform. It handles all business operations including product catalog management, order processing, customer lifecycle, inventory tracking, and customer feedback systems.

### Key Highlights

- **Domain-Driven Design**: Rich domain models with business logic encapsulation
- **Event-Driven Architecture**: Publishes domain events to RabbitMQ for microservice communication
- **Clean Architecture**: Clear separation of concerns across 4 layers
- **CQRS Pattern**: Command Query Responsibility Segregation with MediatR
- **Multi-Schema Database**: Logical separation (Core, Sales, Inventory schemas)
- **RESTful API**: Comprehensive endpoints for all business operations

---

## Business Domain

### Beekeeping Cooperative E-Commerce

This service manages a beekeeping cooperative's complete business operations:

**Products**:
- Raw honey varieties (Wildflower, Clover, Manuka, Acacia, Buckwheat)
- Beeswax products (candles, wraps, balms)
- Beekeeping equipment and supplies
- Gift sets and bundles

**Customers**:
- Customer profile management with loyalty points system
- Address management for shipping
- Order history and preferences
- Feedback and reviews

**Orders**:
- Shopping cart to order conversion
- Multiple payment methods support
- Shipping address management
- Order status tracking (Pending, Processing, Shipped, Delivered, Cancelled)
- Order history and receipts

**Feedback System**:
- Product reviews and ratings
- Order-specific feedback
- Admin response management
- Status workflow (Pending, Reviewed, Responded)

**Inventory Management**:
- Real-time stock tracking
- Low stock alerts with configurable thresholds
- Stock level monitoring and reporting
- Automatic alert generation

---

## Architecture

This project follows **Clean Architecture** principles combined with **Domain-Driven Design**:

```
┌─────────────────────────────────────────────────────────┐
│                     API Layer                            │
│  Controllers, Middleware, Program.cs                     │
│  (HTTP, Routing, Authentication, Error Handling)         │
└───────────────────┬─────────────────────────────────────┘
                    │
┌─────────────────────────────────────────────────────────┐
│                  Application Layer                       │
│  Commands, Queries, DTOs, Validators, Event Handlers     │
│  (CQRS with MediatR, Business Orchestration)            │
└───────────────────┬─────────────────────────────────────┘
                    │
┌─────────────────────────────────────────────────────────┐
│                   Domain Layer                           │
│  Entities, Value Objects, Domain Events, Specifications  │
│  (Business Rules, Domain Logic, Aggregates)             │
└───────────────────┬─────────────────────────────────────┘
                    │
┌─────────────────────────────────────────────────────────┐
│                Infrastructure Layer                      │
│  DbContext, Repositories, Event Publishers               │
│  (Data Access with EF Core, RabbitMQ Integration)       │
└─────────────────────────────────────────────────────────┘
```

### Design Patterns

- **Domain-Driven Design (DDD)**: Rich domain models with business logic
- **CQRS**: Separate commands and queries for better scalability
- **Repository Pattern**: Data access abstraction
- **Unit of Work**: Transaction management with Entity Framework Core
- **Dependency Injection**: Loose coupling and testability
- **Event Sourcing**: Domain events published for event-driven communication
- **Specification Pattern**: Reusable query logic

### Layer Responsibilities

#### API Layer
- RESTful endpoint definitions
- JWT authentication and authorization
- Request/response handling
- Global exception handling middleware
- Swagger/OpenAPI documentation
- CORS configuration

#### Application Layer
- Command handlers for write operations (Create, Update, Delete)
- Query handlers for read operations (Get, List, Search)
- DTO mappings with AutoMapper
- FluentValidation for input validation
- Business workflow orchestration
- Integration event handlers (RabbitMQ consumers)

#### Domain Layer
- **Aggregates**: Customer, Product, Order, Feedback
- **Value Objects**: Money, Address, Rating
- **Domain Events**: OrderCreated, OrderShipped, ProductOutOfStock, FeedbackReceived
- **Business Rules**: Order validation, stock management, pricing logic
- **Specifications**: Filtering and querying logic

#### Infrastructure Layer
- Entity Framework Core DbContext
- Repository implementations
- Database migrations
- RabbitMQ event publisher implementation
- External service integrations

---

## Features

### Product Management

#### Product Catalog
- **CRUD Operations**: Full product lifecycle management
- **Product Types**: Physical goods, digital products, bundles
- **Categories**: Honey, Beeswax, Equipment, Gifts, Supplies
- **Attributes**: Name, description, SKU, price, stock quantity, images
- **Stock Management**: Real-time inventory tracking
- **Pricing**: Decimal precision with currency support (USD)
- **Search & Filter**: By category, type, price range, availability
- **Soft Delete**: Preserve historical data

#### Inventory Tracking
- **Stock Monitoring**: Real-time quantity tracking
- **Threshold Alerts**: Configurable low-stock warnings (default: 10 units)
- **Alert Management**: Track when products fall below threshold
- **Admin Notifications**: Automatic alerts via event system
- **Alert Resolution**: Mark alerts as resolved with admin tracking
- **Stock History**: Audit trail of stock changes

### Order Management

#### Order Processing
- **Order Creation**:
  - Multi-item orders with quantity specification
  - Automatic order number generation (ORD-YYYYMMDD-XXXXX)
  - Stock validation and reservation
  - Price calculation with subtotals
  - Shipping address capture
- **Order Status Workflow**:
  - Pending → Processing → Shipped → Delivered
  - Cancellation support at any stage
  - Status history tracking
- **Payment Methods**: Multiple payment option support
- **Shipping Management**:
  - Full address details (street, city, state, postal code, country)
  - Tracking number support
- **Order Retrieval**:
  - List all orders with pagination
  - Get order by ID with full details
  - Customer order history
  - Admin order dashboard

#### Order Items
- Product association with pricing snapshot
- Quantity and unit price tracking
- Discount application support
- Subtotal calculation per line item
- Currency specification

### Customer Management

#### Customer Profiles
- **Profile Data**:
  - External authentication ID linkage (from AuthService)
  - Full name, email, phone
  - Multiple addresses (street, city, state, postal code, country)
  - Date joined tracking
  - Account status (active/inactive)
- **Loyalty System**: Points accumulation and tracking
- **Customer Lifecycle**: Creation via AuthService events, profile updates
- **Order Association**: Link orders to customer accounts
- **Soft Delete**: Data preservation for compliance

#### Customer Operations
- View customer details
- Update customer information
- View customer order history
- Manage customer addresses
- Track loyalty points

### Feedback & Review System

#### Feedback Management
- **Feedback Submission**:
  - Rating scale (1-5 stars)
  - Written content/review
  - Optional product association
  - Optional order association
  - Customer identity tracking
- **Admin Response**:
  - Respond to feedback
  - Track responder (admin user ID)
  - Response timestamp
- **Status Workflow**:
  - Pending: Newly submitted
  - Reviewed: Admin has seen
  - Responded: Admin has replied
- **Feedback Retrieval**:
  - All feedback (admin)
  - Product-specific feedback
  - Customer feedback history
  - Paginated results

#### Feedback Events
- Publish `feedback.received` event to RabbitMQ
- Trigger notifications to admins
- Trigger acknowledgment to customers

---

## Technology Stack

### Core Technologies
- **ASP.NET Core** - Web API framework
- **C#** - Primary programming language
- **.NET Core Runtime** - Cross-platform runtime

### Data Access & Storage
- **Entity Framework Core** - ORM for data access
- **SQL Server** - Primary database
- **Microsoft.EntityFrameworkCore.SqlServer** - SQL Server provider
- **LINQ** - Query language

### Architecture & Patterns
- **MediatR** - CQRS implementation and in-process messaging
- **FluentValidation** - Request validation framework
- **AutoMapper** - Object-to-object mapping

### Authentication & Security
- **Microsoft.AspNetCore.Authentication.JwtBearer** - JWT token validation
- **BCrypt.Net-Next** - Password hashing (if applicable)
- Role-based authorization (Customer, Admin)

### Integration & Messaging
- **RabbitMQ.Client** - Message queue client for event publishing/consumption
- **Topic Exchange Pattern** - Event routing with routing keys
- **Event-Driven Communication** - Microservice integration

### Logging & Monitoring
- **Serilog** - Structured logging framework
- **Serilog.Sinks.Console** - Console output
- **Serilog.Sinks.File** - File logging with rolling
- **Serilog.Sinks.Elasticsearch** - Centralized log aggregation

### API Documentation
- **Swashbuckle.AspNetCore** - Swagger/OpenAPI generation
- **XML Comments** - API documentation source

---

## Project Structure

```
ShahdCooperative/
│
├── ShahdCooperative.API/
│   ├── Controllers/
│   │   ├── ProductsController.cs         # Product CRUD operations
│   │   ├── OrdersController.cs           # Order management
│   │   ├── CustomersController.cs        # Customer operations
│   │   └── FeedbackController.cs         # Feedback/reviews
│   ├── Middleware/
│   │   └── ExceptionHandlingMiddleware.cs
│   ├── Program.cs                         # Application startup
│   └── appsettings.json                   # Configuration
│
├── ShahdCooperative.Application/
│   ├── Commands/
│   │   ├── Products/
│   │   │   ├── CreateProduct/
│   │   │   ├── UpdateProduct/
│   │   │   └── DeleteProduct/
│   │   ├── Orders/
│   │   │   ├── CreateOrder/
│   │   │   ├── UpdateOrderStatus/
│   │   │   └── CancelOrder/
│   │   ├── Customers/
│   │   │   ├── CreateCustomer/
│   │   │   └── UpdateCustomer/
│   │   └── Feedback/
│   │       ├── SubmitFeedback/
│   │       └── RespondToFeedback/
│   ├── Queries/
│   │   ├── Products/
│   │   │   ├── GetAllProducts/
│   │   │   └── GetProductById/
│   │   ├── Orders/
│   │   │   ├── GetAllOrders/
│   │   │   └── GetOrderById/
│   │   ├── Customers/
│   │   │   └── GetCustomerById/
│   │   └── Feedback/
│   │       └── GetAllFeedback/
│   ├── DTOs/                              # Data Transfer Objects
│   ├── EventHandlers/                     # RabbitMQ event consumers
│   │   ├── UserRegisteredEventHandler.cs
│   │   └── UserUpdatedEventHandler.cs
│   └── DependencyInjection.cs
│
├── ShahdCooperative.Domain/
│   ├── Entities/
│   │   ├── Customer.cs                    # Customer aggregate root
│   │   ├── Product.cs                     # Product aggregate root
│   │   ├── Order.cs                       # Order aggregate root
│   │   ├── OrderItem.cs                   # Order line items
│   │   ├── Feedback.cs                    # Feedback entity
│   │   └── InventoryAlert.cs              # Stock alert entity
│   ├── ValueObjects/
│   │   ├── Money.cs                       # Price representation
│   │   └── Address.cs                     # Shipping/billing address
│   ├── Events/
│   │   ├── OrderCreatedEvent.cs
│   │   ├── OrderShippedEvent.cs
│   │   ├── ProductOutOfStockEvent.cs
│   │   └── FeedbackReceivedEvent.cs
│   ├── Interfaces/
│   │   ├── IProductRepository.cs
│   │   ├── IOrderRepository.cs
│   │   ├── ICustomerRepository.cs
│   │   ├── IFeedbackRepository.cs
│   │   └── IMessagePublisher.cs
│   ├── Enums/
│   │   ├── OrderStatus.cs                 # Order workflow states
│   │   ├── ProductCategory.cs             # Product categorization
│   │   ├── ProductType.cs                 # Physical, Digital, Bundle
│   │   └── FeedbackStatus.cs              # Feedback workflow
│   └── Specifications/
│       └── ProductSpecifications.cs       # Reusable query logic
│
├── ShahdCooperative.Infrastructure/
│   ├── Data/
│   │   ├── ApplicationDbContext.cs        # EF Core DbContext
│   │   └── Migrations/                    # Database migrations
│   ├── Repositories/
│   │   ├── ProductRepository.cs
│   │   ├── OrderRepository.cs
│   │   ├── CustomerRepository.cs
│   │   └── FeedbackRepository.cs
│   ├── Services/
│   │   └── RabbitMQPublisher.cs           # Event publishing service
│   └── DependencyInjection.cs
│
└── Tests/
    ├── ShahdCooperative.API.Tests/
    ├── ShahdCooperative.Application.Tests/
    └── ShahdCooperative.Domain.Tests/
```

---

## API Endpoints

### Product Endpoints

```http
GET /api/products
GET /api/products/{id}
POST /api/products                # Admin only
PUT /api/products/{id}            # Admin only
DELETE /api/products/{id}         # Admin only (soft delete)
```

**Example: Create Product**
```http
POST /api/products
Authorization: Bearer {adminToken}
Content-Type: application/json

{
  "name": "Raw Wildflower Honey",
  "description": "Pure, unfiltered wildflower honey from local apiaries",
  "category": "Honey",
  "type": "Physical",
  "sku": "HONEY-WF-500",
  "price": 15.99,
  "currency": "USD",
  "stockQuantity": 100,
  "thresholdLevel": 10,
  "imageUrl": "https://cdn.example.com/honey-wildflower.jpg"
}

Response: 201 Created
{
  "id": "guid",
  "name": "Raw Wildflower Honey",
  "sku": "HONEY-WF-500",
  "price": 15.99,
  "stockQuantity": 100,
  "isActive": true,
  "createdAt": "2025-11-08T10:30:00Z"
}
```

### Order Endpoints

```http
GET /api/orders                   # All orders (Admin) or user orders (Customer)
GET /api/orders/{id}
POST /api/orders                  # Create new order
PUT /api/orders/{id}/status       # Update order status (Admin)
DELETE /api/orders/{id}           # Cancel order
```

**Example: Create Order**
```http
POST /api/orders
Authorization: Bearer {customerToken}
Content-Type: application/json

{
  "items": [
    {
      "productId": "product-guid-1",
      "quantity": 2
    },
    {
      "productId": "product-guid-2",
      "quantity": 1
    }
  ],
  "shippingAddress": {
    "street": "123 Main St",
    "city": "Springfield",
    "state": "IL",
    "postalCode": "62701",
    "country": "USA"
  },
  "paymentMethod": "CreditCard",
  "notes": "Please leave at front door"
}

Response: 201 Created
{
  "id": "order-guid",
  "orderNumber": "ORD-20251108-00042",
  "customerId": "customer-guid",
  "status": "Pending",
  "totalAmount": 47.97,
  "currency": "USD",
  "items": [...],
  "shippingAddress": {...},
  "orderDate": "2025-11-08T14:22:00Z"
}
```

### Customer Endpoints

```http
GET /api/customers/{id}
PUT /api/customers/{id}
GET /api/customers/{id}/orders
```

### Feedback Endpoints

```http
GET /api/feedback                 # All feedback (Admin) or user feedback (Customer)
POST /api/feedback                # Submit feedback
POST /api/feedback/{id}/respond   # Admin response
```

**Example: Submit Feedback**
```http
POST /api/feedback
Authorization: Bearer {customerToken}
Content-Type: application/json

{
  "productId": "product-guid",
  "orderId": "order-guid",
  "rating": 5,
  "content": "Excellent quality honey! The wildflower variety has amazing flavor."
}

Response: 201 Created
{
  "id": "feedback-guid",
  "customerId": "customer-guid",
  "rating": 5,
  "content": "Excellent quality honey! The wildflower variety has amazing flavor.",
  "status": "Pending",
  "createdAt": "2025-11-08T16:45:00Z"
}
```

---

## Integration

### Event-Driven Communication

This service integrates with other microservices via RabbitMQ events:

#### Events Consumed

**From AuthService:**
- `user.registered` → Creates Customer record
- `user.updated` → Updates Customer information

#### Events Published

**To NotificationService:**
- `order.created` → Sends order confirmation email
- `order.shipped` → Sends shipping notification
- `product.out-of-stock` → Alerts admin of low inventory
- `feedback.received` → Notifies admin of new feedback

#### RabbitMQ Configuration

- **Exchange**: `shahdcooperative.events` (topic exchange)
- **Routing Keys**: Hierarchical dot-notation (e.g., `order.created`, `product.out-of-stock`)
- **Message Format**: JSON serialization
- **Durability**: Persistent messages for reliability

### Authentication Integration

- **JWT Token Validation**: Validates tokens issued by AuthService
- **Shared Secret**: Common JWT signing key across services
- **Claims**: Extracts User ID, Email, Role from token
- **Authorization**: Role-based access control (Customer, Admin)

---

## Getting Started

### Prerequisites

- **.NET SDK** (version 6.0 or higher)
- **SQL Server** (2019 or higher, Express edition works)
- **RabbitMQ** (for event-driven integration)
- **Visual Studio 2022** / **VS Code** / **Rider**

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/Omar1Ach/ShahdCooperative.git
cd ShahdCooperative
```

2. **Configure Database Connection**

Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ShahdCooperative;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3. **Run Database Migrations**
```bash
dotnet ef database update --project ShahdCooperative.Infrastructure
```

4. **Configure RabbitMQ**

Update `appsettings.json`:
```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "Exchange": "shahdcooperative.events"
  }
}
```

5. **Configure JWT Authentication**
```json
{
  "JwtSettings": {
    "SecretKey": "your-shared-secret-key-minimum-32-characters",
    "Issuer": "ShahdCooperativeAuthService",
    "Audience": "ShahdCooperativeAPI",
    "ExpirationMinutes": 60
  }
}
```

6. **Restore Dependencies**
```bash
dotnet restore
```

7. **Build the Project**
```bash
dotnet build
```

8. **Run the Application**
```bash
dotnet run --project ShahdCooperative.API
```

9. **Access Swagger UI**
```
https://localhost:5001/swagger
```

---

## Configuration

### Application Settings

**Database Configuration:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ShahdCooperative;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**JWT Configuration:**
```json
{
  "JwtSettings": {
    "SecretKey": "must-match-authservice-secret-key",
    "Issuer": "ShahdCooperativeAuthService",
    "Audience": "ShahdCooperativeAPI",
    "ExpirationMinutes": 60
  }
}
```

**RabbitMQ Configuration:**
```json
{
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "Exchange": "shahdcooperative.events"
  }
}
```

**Logging Configuration:**
```json
{
  "Serilog": {
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
  }
}
```

**Elasticsearch Configuration (Optional):**
```json
{
  "ElasticsearchSettings": {
    "Uri": "http://localhost:9200",
    "IndexFormat": "shahdcooperative-logs-{0:yyyy.MM.dd}",
    "Enabled": true
  }
}
```

---

## Business Rules

### Order Processing Rules

1. **Stock Validation**: Order creation fails if insufficient stock
2. **Price Snapshot**: Order items capture current product price
3. **Order Number Format**: `ORD-YYYYMMDD-XXXXX` (auto-generated)
4. **Status Transitions**: Pending → Processing → Shipped → Delivered
5. **Cancellation**: Only pending/processing orders can be cancelled

### Inventory Management Rules

1. **Low Stock Threshold**: Default 10 units (configurable per product)
2. **Alert Generation**: Automatic when stock falls below threshold
3. **Stock Reservation**: Deducted on order creation, restored on cancellation
4. **Negative Stock**: Prevented at database and application level

### Feedback Rules

1. **Rating Range**: 1-5 stars only
2. **Product/Order Association**: At least one required
3. **Customer Verification**: Only authenticated customers can submit
4. **Admin Response**: Only admins can respond to feedback
5. **Status Workflow**: Pending → Reviewed → Responded

---

## Developer Notes

### Database Schemas

- **Core Schema**: Customers, Feedback
- **Sales Schema**: Orders, OrderItems
- **Inventory Schema**: Products, InventoryAlerts
- **Security Schema**: Users, RefreshTokens (managed by AuthService)
- **Notification Schema**: NotificationQueue, Templates (managed by NotificationService)

### Entity Framework Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project ShahdCooperative.Infrastructure

# Update database
dotnet ef database update --project ShahdCooperative.Infrastructure

# Remove last migration (if not applied)
dotnet ef migrations remove --project ShahdCooperative.Infrastructure
```

### Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

---

## Performance Considerations

- **Entity Framework Core**: Optimized queries with Include/ThenInclude
- **AsNoTracking**: Used for read-only queries
- **Pagination**: Large result sets paginated to reduce memory
- **Async/Await**: All I/O operations are asynchronous
- **Connection Pooling**: SQL Server connection pooling enabled
- **Caching**: Consider Redis for frequently accessed data (future enhancement)

---

## License

**Proprietary License** - Copyright © 2025 ShahdCooperative
All rights reserved. Unauthorized copying, modification, distribution, or use of this software is strictly prohibited.

---

## Developer

**Omar Achbani**
Full-Stack React .NET Developer

- GitHub: [@Omar1Ach](https://github.com/Omar1Ach)
- Project: [ShahdCooperative Main Service](https://github.com/Omar1Ach/ShahdCooperative)

---

**Built for ShahdCooperative Beekeeping Cooperative**
