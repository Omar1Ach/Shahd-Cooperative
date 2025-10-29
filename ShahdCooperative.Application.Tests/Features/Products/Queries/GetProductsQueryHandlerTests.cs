using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Application.Features.Products.Queries.GetProducts;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Products.Queries;

public class GetProductsQueryHandlerTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetProductsQueryHandler _handler;

    public GetProductsQueryHandlerTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetProductsQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllProducts()
    {
        // Arrange
        var query = new GetProductsQuery();
        var products = new List<Product>
        {
            Product.Create("Honey", "HON-001", "Honey", ProductType.Honey, 19.99m, "USD", 100, 10),
            Product.Create("Beeswax", "BEE-001", "Wax", ProductType.BeeswaxProduct, 14.99m, "USD", 50, 5)
        };

        var productDtos = new List<ProductDto>
        {
            new ProductDto { Id = Guid.NewGuid(), Name = "Honey", Price = 19.99m },
            new ProductDto { Id = Guid.NewGuid(), Name = "Beeswax", Price = 14.99m }
        };

        _mockRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        _mockMapper.Setup(x => x.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
            .Returns(productDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
        _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoProducts_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetProductsQuery();
        var products = new List<Product>();
        var productDtos = new List<ProductDto>();

        _mockRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(products);
        _mockMapper.Setup(x => x.Map<IEnumerable<ProductDto>>(It.IsAny<IEnumerable<Product>>()))
            .Returns(productDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }
}
