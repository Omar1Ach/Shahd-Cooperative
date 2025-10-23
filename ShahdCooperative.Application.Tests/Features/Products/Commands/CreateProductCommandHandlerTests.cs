using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Application.Features.Products.Commands.CreateProduct;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Products.Commands;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new CreateProductCommandHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var dto = new CreateProductDto
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

        var command = new CreateProductCommand(dto);
        var product = new Mock<Product>().Object;
        var productDto = new ProductDto { Id = Guid.NewGuid(), Name = dto.Name, Price = dto.Price };

        _mockMapper.Setup(x => x.Map<Product>(It.IsAny<CreateProductDto>()))
            .Returns(product);
        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockMapper.Setup(x => x.Map<ProductDto>(It.IsAny<Product>()))
            .Returns(productDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(dto.Name, result.Value.Name);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
