using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using ShahdCooperative.Application.DTOs.Products;

namespace ShahdCooperative.IntegrationTests.Controllers;

public class ProductsControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreated()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            Name = "Integration Test Honey",
            SKU = "INT-001",
            Description = "Test honey product",
            Category = "Honey",
            Type = "Honey",
            Price = 25.99m,
            Currency = "USD",
            StockQuantity = 50,
            ThresholdLevel = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", createProductDto);

        // Assert
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with status {response.StatusCode}. Error: {errorContent}");
        }
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be("Integration Test Honey");
        createdProduct.SKU.Should().Be("INT-001");
        createdProduct.Price.Should().Be(25.99m);
    }

    [Fact]
    public async Task GetProductById_WithValidId_ReturnsProduct()
    {
        // Arrange - Create a product first
        var createDto = new CreateProductDto
        {
            Name = "Test Beeswax",
            SKU = "INT-002",
            Category = "Wax",
            Type = "Honey",
            Price = 15.99m,
            Currency = "USD",
            StockQuantity = 30,
            ThresholdLevel = 5
        };

        var createResponse = await _client.PostAsJsonAsync("/api/products", createDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await _client.GetAsync($"/api/products/{createdProduct!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var product = await response.Content.ReadFromJsonAsync<ProductDto>();
        product.Should().NotBeNull();
        product!.Name.Should().Be("Test Beeswax");
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsSuccess()
    {
        // Arrange - Create a product first
        var createDto = new CreateProductDto
        {
            Name = "Original Honey",
            SKU = "INT-003",
            Category = "Honey",
            Type = "Honey",
            Price = 20.00m,
            Currency = "USD",
            StockQuantity = 100,
            ThresholdLevel = 10
        };

        var createResponse = await _client.PostAsJsonAsync("/api/products", createDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        var updateDto = new UpdateProductDto
        {
            Id = createdProduct!.Id,
            Name = "Updated Honey",
            Description = "Updated description",
            Category = "Honey",
            Price = 22.50m,
            Currency = "USD",
            StockQuantity = 80,
            ThresholdLevel = 10
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/api/products/{createdProduct.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedProduct = await response.Content.ReadFromJsonAsync<ProductDto>();
        updatedProduct.Should().NotBeNull();
        updatedProduct!.Name.Should().Be("Updated Honey");
        updatedProduct.Price.Should().Be(22.50m);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent()
    {
        // Arrange - Create a product first
        var createDto = new CreateProductDto
        {
            Name = "To Delete Honey",
            SKU = "INT-004",
            Category = "Honey",
            Type = "Honey",
            Price = 10.00m,
            Currency = "USD",
            StockQuantity = 10,
            ThresholdLevel = 2
        };

        var createResponse = await _client.PostAsJsonAsync("/api/products", createDto);
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();

        // Act
        var response = await _client.DeleteAsync($"/api/products/{createdProduct!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify product is deleted
        var getResponse = await _client.GetAsync($"/api/products/{createdProduct.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
