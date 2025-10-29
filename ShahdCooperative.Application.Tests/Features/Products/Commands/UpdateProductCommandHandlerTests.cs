using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Application.Events;
using ShahdCooperative.Application.Features.Products.Commands.UpdateProduct;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Interfaces;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Products.Commands;

public class UpdateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly UpdateProductCommandHandler _handler;

    public UpdateProductCommandHandlerTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _handler = new UpdateProductCommandHandler(_mockRepository.Object, _mockMapper.Object, _mockEventPublisher.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var dto = new UpdateProductDto
        {
            Id = productId,
            Name = "Updated Honey",
            Description = "Updated description",
            Category = "Honey",
            Price = 24.99m,
            Currency = "USD",
            StockQuantity = 50,
            ThresholdLevel = 5
        };

        var command = new UpdateProductCommand(dto);
        var product = Product.Create(
            name: "Original Honey",
            sku: "HON-001",
            category: "Honey",
            type: ProductType.BeeProduct,
            price: 19.99m,
            currency: "USD",
            stockQuantity: 100,
            thresholdLevel: 10);
        var productDto = new ProductDto { Id = productId, Name = dto.Name, Price = dto.Price };

        _mockRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockMapper.Setup(x => x.Map(It.IsAny<UpdateProductDto>(), It.IsAny<Product>()))
            .Returns(product);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockMapper.Setup(x => x.Map<ProductDto>(It.IsAny<Product>()))
            .Returns(productDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(dto.Name, result.Value.Name);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ReturnsFailureResult()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var dto = new UpdateProductDto
        {
            Id = productId,
            Name = "Updated Honey",
            Category = "Honey",
            Price = 24.99m
        };

        var command = new UpdateProductCommand(dto);

        _mockRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
