using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ShahdCooperative.API.Controllers;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Application.Features.Products.Commands.CreateProduct;
using ShahdCooperative.Application.Features.Products.Commands.DeleteProduct;
using ShahdCooperative.Application.Features.Products.Commands.UpdateProduct;
using ShahdCooperative.Application.Features.Products.Queries.GetProductById;
using ShahdCooperative.Application.Features.Products.Queries.GetProducts;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.API.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<ProductsController>> _mockLogger;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<ProductsController>>();
        _controller = new ProductsController(_mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetProducts_ReturnsOkResult_WithProducts()
    {
        // Arrange
        var products = new List<ProductDto>
        {
            new ProductDto { Id = Guid.NewGuid(), Name = "Honey", Price = 19.99m },
            new ProductDto { Id = Guid.NewGuid(), Name = "Beeswax", Price = 14.99m }
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<GetProductsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IEnumerable<ProductDto>>.Success(products));

        // Act
        var result = await _controller.GetProducts(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductDto>>(okResult.Value);
        Assert.Equal(2, returnedProducts.Count());
    }

    [Fact]
    public async Task GetProduct_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var productDto = new ProductDto { Id = productId, Name = "Honey", Price = 19.99m };

        _mockMediator.Setup(x => x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProductDto>.Success(productDto));

        // Act
        var result = await _controller.GetProduct(productId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(productId, returnedProduct.Id);
    }

    [Fact]
    public async Task GetProduct_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockMediator.Setup(x => x.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProductDto>.Failure("Product not found", "NOT_FOUND"));

        // Act
        var result = await _controller.GetProduct(productId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateProduct_ValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateProductDto
        {
            Name = "Test Honey",
            Description = "Test description",
            SKU = "TEST-001",
            Category = "Honey",
            Type = "Equipment",
            Price = 19.99m,
            Currency = "USD",
            StockQuantity = 100,
            ThresholdLevel = 10
        };

        var productDto = new ProductDto
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Price = createDto.Price
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProductDto>.Success(productDto));

        // Act
        var result = await _controller.CreateProduct(createDto, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedProduct = Assert.IsType<ProductDto>(createdResult.Value);
        Assert.Equal(productDto.Id, returnedProduct.Id);
    }

    [Fact]
    public async Task UpdateProduct_ValidDto_ReturnsOkResult()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var updateDto = new UpdateProductDto
        {
            Id = productId,
            Name = "Updated Honey",
            Category = "Honey",
            Price = 24.99m
        };

        var productDto = new ProductDto
        {
            Id = productId,
            Name = updateDto.Name,
            Price = updateDto.Price
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ProductDto>.Success(productDto));

        // Act
        var result = await _controller.UpdateProduct(productId, updateDto, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedProduct = Assert.IsType<ProductDto>(okResult.Value);
        Assert.Equal(updateDto.Name, returnedProduct.Name);
    }

    [Fact]
    public async Task UpdateProduct_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var differentId = Guid.NewGuid();
        var updateDto = new UpdateProductDto
        {
            Id = differentId,
            Name = "Updated Honey",
            Category = "Honey",
            Price = 24.99m
        };

        // Act
        var result = await _controller.UpdateProduct(productId, updateDto, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockMediator.Setup(x => x.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.DeleteProduct(productId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();

        _mockMediator.Setup(x => x.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Product not found", "NOT_FOUND"));

        // Act
        var result = await _controller.DeleteProduct(productId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
