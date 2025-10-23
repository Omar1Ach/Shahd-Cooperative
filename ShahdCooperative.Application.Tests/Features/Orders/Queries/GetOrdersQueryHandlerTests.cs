using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Application.Features.Orders.Queries.GetOrders;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Orders.Queries;

public class GetOrdersQueryHandlerTests
{
    private readonly Mock<IOrderRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetOrdersQueryHandler _handler;

    public GetOrdersQueryHandlerTests()
    {
        _mockRepository = new Mock<IOrderRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetOrdersQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllOrders()
    {
        // Arrange
        var query = new GetOrdersQuery();
        var customerId = Guid.NewGuid();
        var orders = new List<Order>
        {
            Order.Create(customerId, "USD", "Street 1", "City 1", "State 1", "10001", "USA"),
            Order.Create(customerId, "USD", "Street 2", "City 2", "State 2", "20002", "USA")
        };

        var orderDtos = new List<OrderDto>
        {
            new OrderDto { Id = Guid.NewGuid(), CustomerId = customerId },
            new OrderDto { Id = Guid.NewGuid(), CustomerId = customerId }
        };

        _mockRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        _mockMapper.Setup(x => x.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()))
            .Returns(orderDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
        _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoOrders_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetOrdersQuery();
        var orders = new List<Order>();
        var orderDtos = new List<OrderDto>();

        _mockRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(orders);
        _mockMapper.Setup(x => x.Map<IEnumerable<OrderDto>>(It.IsAny<IEnumerable<Order>>()))
            .Returns(orderDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }
}
