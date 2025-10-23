using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Application.Features.Orders.Queries.GetOrderById;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Orders.Queries;

public class GetOrderByIdQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetOrderByIdQueryHandler _handler;

    public GetOrderByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetOrderByIdQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_OrderExists_ReturnsSuccessResult()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);
        var order = Order.Create(customerId, "USD", "123 Main St", "New York", "NY", "10001", "USA");
        var orderDto = new OrderDto { Id = orderId, CustomerId = customerId };

        _mockRepository.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _mockMapper.Setup(x => x.Map<OrderDto>(It.IsAny<Order>()))
            .Returns(orderDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(orderId, result.Value.Id);
        _mockRepository.Verify(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_OrderNotFound_ReturnsFailureResult()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderByIdQuery(orderId);

        _mockRepository.Setup(x => x.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
        Assert.Null(result.Value);
    }
}
