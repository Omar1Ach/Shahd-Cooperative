using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Application.DTOs.Products;

namespace ShahdCooperative.IntegrationTests.Controllers;

public class OrdersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrdersControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ReturnsCreated()
    {
        // Arrange - Create a customer first
        var createCustomerDto = new CreateCustomerDto
        {
            ExternalAuthId = "auth-test-001",
            Name = "Test Customer",
            Email = "test@example.com"
        };

        var customerResponse = await _client.PostAsJsonAsync("/api/customers", createCustomerDto);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();

        // Create a product
        var createProductDto = new CreateProductDto
        {
            Name = "Order Test Honey",
            SKU = "ORD-001",
            Category = "Honey",
            Type = "Honey",
            Price = 30.00m,
            Currency = "USD",
            StockQuantity = 100,
            ThresholdLevel = 10
        };

        var productResponse = await _client.PostAsJsonAsync("/api/products", createProductDto);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Create order DTO
        var createOrderDto = new CreateOrderDto
        {
            CustomerId = customer!.Id,
            ShippingStreet = "123 Test St",
            ShippingCity = "Test City",
            ShippingState = "TS",
            ShippingPostalCode = "12345",
            ShippingCountry = "USA",
            OrderItems = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { ProductId = product!.Id, Quantity = 2 }
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", createOrderDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdOrder = await response.Content.ReadFromJsonAsync<OrderDto>();
        createdOrder.Should().NotBeNull();
        createdOrder!.CustomerId.Should().Be(customer.Id);
        createdOrder.TotalAmount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetOrders_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/orders");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task GetOrderById_WithValidId_ReturnsOrder()
    {
        // Arrange - Create customer, product, and order
        var customerDto = new CreateCustomerDto
        {
            ExternalAuthId = "auth-test-002",
            Name = "Another Customer",
            Email = "another@example.com"
        };
        var customerResponse = await _client.PostAsJsonAsync("/api/customers", customerDto);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var productDto = new CreateProductDto
        {
            Name = "Test Product",
            SKU = "ORD-002",
            Category = "Honey",
            Type = "Honey",
            Price = 15.00m,
            Currency = "USD",
            StockQuantity = 50,
            ThresholdLevel = 5
        };
        var productResponse = await _client.PostAsJsonAsync("/api/products", productDto);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();

        var orderDto = new CreateOrderDto
        {
            CustomerId = customer!.Id,
            ShippingStreet = "456 Test Ave",
            ShippingCity = "Test Town",
            ShippingState = "TT",
            ShippingPostalCode = "54321",
            ShippingCountry = "USA",
            OrderItems = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { ProductId = product!.Id, Quantity = 1 }
            }
        };
        var createResponse = await _client.PostAsJsonAsync("/api/orders", orderDto);
        var createdOrder = await createResponse.Content.ReadFromJsonAsync<OrderDto>();

        // Act
        var response = await _client.GetAsync($"/api/orders/{createdOrder!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var order = await response.Content.ReadFromJsonAsync<OrderDto>();
        order.Should().NotBeNull();
        order!.Id.Should().Be(createdOrder.Id);
    }

    [Fact]
    public async Task CreateOrder_WithInvalidCustomer_ReturnsBadRequest()
    {
        // Arrange
        var createOrderDto = new CreateOrderDto
        {
            CustomerId = Guid.NewGuid(), // Non-existent customer
            ShippingStreet = "123 Test St",
            ShippingCity = "Test City",
            ShippingState = "TS",
            ShippingPostalCode = "12345",
            ShippingCountry = "USA",
            OrderItems = new List<CreateOrderItemDto>()
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/orders", createOrderDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
