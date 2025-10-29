using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShahdCooperative.Application.DTOs.Customers;

namespace ShahdCooperative.IntegrationTests.Controllers;

public class CustomersControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CustomersControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCustomers_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/customers");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task CreateCustomer_WithValidData_ReturnsCreated()
    {
        // Arrange
        var createCustomerDto = new CreateCustomerDto
        {
            ExternalAuthId = "auth-integration-001",
            Name = "Integration Test Customer",
            Email = "integration@example.com",
            Phone = "+1234567890",
            Street = "123 Test Street",
            City = "Test City",
            State = "TS",
            PostalCode = "12345",
            Country = "USA"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/customers", createCustomerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdCustomer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        createdCustomer.Should().NotBeNull();
        createdCustomer!.Name.Should().Be("Integration Test Customer");
        createdCustomer.Email.Should().Be("integration@example.com");
        createdCustomer.LoyaltyPoints.Should().Be(0);
    }

    [Fact]
    public async Task GetCustomerById_WithValidId_ReturnsCustomer()
    {
        // Arrange - Create a customer first
        var createDto = new CreateCustomerDto
        {
            ExternalAuthId = "auth-integration-002",
            Name = "Test Customer 2",
            Email = "test2@example.com",
            Phone = "+0987654321"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/customers", createDto);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();

        // Act
        var response = await _client.GetAsync($"/api/customers/{createdCustomer!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var customer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        customer.Should().NotBeNull();
        customer!.Id.Should().Be(createdCustomer.Id);
        customer.Name.Should().Be("Test Customer 2");
    }

    [Fact]
    public async Task UpdateCustomer_WithValidData_ReturnsSuccess()
    {
        // Arrange - Create a customer first
        var createDto = new CreateCustomerDto
        {
            ExternalAuthId = "auth-integration-003",
            Name = "Original Name",
            Email = "original@example.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/customers", createDto);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();

        var updateDto = new UpdateCustomerDto
        {
            Id = createdCustomer!.Id,
            Name = "Updated Name",
            Email = "updated@example.com",
            Phone = "+1111111111",
            Street = "456 Updated Street",
            City = "Updated City",
            State = "UC",
            PostalCode = "54321",
            Country = "USA"
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/customers/{createdCustomer.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedCustomer = await response.Content.ReadFromJsonAsync<CustomerDto>();
        updatedCustomer.Should().NotBeNull();
        updatedCustomer!.Name.Should().Be("Updated Name");
        updatedCustomer.Email.Should().Be("updated@example.com");
    }

    [Fact]
    public async Task DeleteCustomer_WithValidId_ReturnsNoContent()
    {
        // Arrange - Create a customer first
        var createDto = new CreateCustomerDto
        {
            ExternalAuthId = "auth-integration-004",
            Name = "To Delete Customer",
            Email = "delete@example.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/customers", createDto);
        var createdCustomer = await createResponse.Content.ReadFromJsonAsync<CustomerDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/customers/{createdCustomer!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify customer is deleted
        var getResponse = await _client.GetAsync($"/api/customers/{createdCustomer.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCustomerById_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync($"/api/customers/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCustomer_WithIdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var updateDto = new UpdateCustomerDto
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Email = "test@test.com"
        };

        // Act - Use different ID in route
        var response = await _client.PutAsJsonAsync($"/api/customers/{Guid.NewGuid()}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
