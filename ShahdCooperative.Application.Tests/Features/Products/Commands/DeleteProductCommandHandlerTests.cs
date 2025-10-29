using Moq;
using ShahdCooperative.Application.Features.Products.Commands.DeleteProduct;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Products.Commands;

public class DeleteProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly DeleteProductCommandHandler _handler;

    public DeleteProductCommandHandlerTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _handler = new DeleteProductCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);
        var product = Product.Create(
            name: "Test Honey",
            sku: "HON-001",
            category: "Honey",
            type: ProductType.BeeProduct,
            price: 19.99m,
            currency: "USD",
            stockQuantity: 100,
            thresholdLevel: 10);

        _mockRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ReturnsFailureResult()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var command = new DeleteProductCommand(productId);

        _mockRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
