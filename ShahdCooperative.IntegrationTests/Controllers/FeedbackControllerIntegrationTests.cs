using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Application.DTOs.Products;

namespace ShahdCooperative.IntegrationTests.Controllers;

public class FeedbackControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FeedbackControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetFeedback_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/feedback");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task CreateFeedback_WithValidData_ReturnsCreated()
    {
        // Arrange - Create customer and product first
        var customerDto = new CreateCustomerDto
        {
            ExternalAuthId = "auth-feedback-001",
            Name = "Feedback Test Customer",
            Email = "feedback@example.com"
        };
        var customerResponse = await _client.PostAsJsonAsync("/api/customers", customerDto);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var productDto = new CreateProductDto
        {
            Name = "Feedback Test Honey",
            SKU = "FBK-001",
            Category = "Honey",
            Type = "Honey",
            Price = 25.00m,
            Currency = "USD",
            StockQuantity = 50,
            ThresholdLevel = 10
        };
        var productResponse = await _client.PostAsJsonAsync("/api/products", productDto);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();

        var createFeedbackDto = new CreateFeedbackDto
        {
            CustomerId = customer!.Id,
            ProductId = product!.Id,
            Rating = 5,
            Content = "Excellent honey, very delicious!"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/feedback", createFeedbackDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdFeedback = await response.Content.ReadFromJsonAsync<FeedbackDto>();
        createdFeedback.Should().NotBeNull();
        createdFeedback!.Rating.Should().Be(5);
        createdFeedback.Content.Should().Be("Excellent honey, very delicious!");
        createdFeedback.CustomerId.Should().Be(customer.Id);
    }

    [Fact]
    public async Task GetFeedbackById_WithValidId_ReturnsFeedback()
    {
        // Arrange - Create customer, product, and feedback
        var customerDto = new CreateCustomerDto
        {
            ExternalAuthId = "auth-feedback-002",
            Name = "Feedback Customer 2",
            Email = "feedback2@example.com"
        };
        var customerResponse = await _client.PostAsJsonAsync("/api/customers", customerDto);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var productDto = new CreateProductDto
        {
            Name = "Test Beeswax",
            SKU = "FBK-002",
            Category = "Wax",
            Type = "BeeswaxProduct",
            Price = 15.00m,
            Currency = "USD",
            StockQuantity = 30,
            ThresholdLevel = 5
        };
        var productResponse = await _client.PostAsJsonAsync("/api/products", productDto);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();

        var feedbackDto = new CreateFeedbackDto
        {
            CustomerId = customer!.Id,
            ProductId = product!.Id,
            Rating = 4,
            Content = "Good quality beeswax"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/feedback", feedbackDto);
        var createdFeedback = await createResponse.Content.ReadFromJsonAsync<FeedbackDto>();

        // Act
        var response = await _client.GetAsync($"/api/feedback/{createdFeedback!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var feedback = await response.Content.ReadFromJsonAsync<FeedbackDto>();
        feedback.Should().NotBeNull();
        feedback!.Id.Should().Be(createdFeedback.Id);
        feedback.Rating.Should().Be(4);
    }

    [Fact]
    public async Task DeleteFeedback_WithValidId_ReturnsNoContent()
    {
        // Arrange - Create customer, product, and feedback
        var customerDto = new CreateCustomerDto
        {
            ExternalAuthId = "auth-feedback-003",
            Name = "Delete Feedback Customer",
            Email = "deletefeedback@example.com"
        };
        var customerResponse = await _client.PostAsJsonAsync("/api/customers", customerDto);
        var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var productDto = new CreateProductDto
        {
            Name = "Delete Test Product",
            SKU = "FBK-003",
            Category = "Equipment",
            Type = "Equipment",
            Price = 50.00m,
            Currency = "USD",
            StockQuantity = 20,
            ThresholdLevel = 3
        };
        var productResponse = await _client.PostAsJsonAsync("/api/products", productDto);
        var product = await productResponse.Content.ReadFromJsonAsync<ProductDto>();

        var feedbackDto = new CreateFeedbackDto
        {
            CustomerId = customer!.Id,
            ProductId = product!.Id,
            Rating = 3,
            Content = "To be deleted"
        };
        var createResponse = await _client.PostAsJsonAsync("/api/feedback", feedbackDto);
        var createdFeedback = await createResponse.Content.ReadFromJsonAsync<FeedbackDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/feedback/{createdFeedback!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify feedback is deleted
        var getResponse = await _client.GetAsync($"/api/feedback/{createdFeedback.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetFeedbackById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/feedback/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateFeedback_WithInvalidCustomer_ReturnsNotFound()
    {
        // Arrange
        var createFeedbackDto = new CreateFeedbackDto
        {
            CustomerId = Guid.NewGuid(), // Non-existent customer
            ProductId = Guid.NewGuid(),
            Rating = 5,
            Content = "Test"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/feedback", createFeedbackDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
