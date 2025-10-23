using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ShahdCooperative.API.Controllers;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Application.Features.Orders.Commands.CreateOrder;
using ShahdCooperative.Application.Features.Orders.Queries.GetOrderById;
using ShahdCooperative.Application.Features.Orders.Queries.GetOrders;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.API.Tests.Controllers;

public class OrdersControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<OrdersController>> _mockLogger;
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<OrdersController>>();
        _controller = new OrdersController(_mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetOrders_ReturnsOkResult_WithOrders()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var orders = new List<OrderDto>
        {
            new OrderDto { Id = Guid.NewGuid(), CustomerId = customerId },
            new OrderDto { Id = Guid.NewGuid(), CustomerId = customerId }
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<GetOrdersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IEnumerable<OrderDto>>.Success(orders));

        // Act
        var result = await _controller.GetOrders(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedOrders = Assert.IsAssignableFrom<IEnumerable<OrderDto>>(okResult.Value);
        Assert.Equal(2, returnedOrders.Count());
    }

    [Fact]
    public async Task GetOrder_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var orderDto = new OrderDto { Id = orderId, CustomerId = customerId };

        _mockMediator.Setup(x => x.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OrderDto>.Success(orderDto));

        // Act
        var result = await _controller.GetOrder(orderId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedOrder = Assert.IsType<OrderDto>(okResult.Value);
        Assert.Equal(orderId, returnedOrder.Id);
    }

    [Fact]
    public async Task GetOrder_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _mockMediator.Setup(x => x.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OrderDto>.Failure("Order not found", "NOT_FOUND"));

        // Act
        var result = await _controller.GetOrder(orderId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateOrder_ValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var createDto = new CreateOrderDto
        {
            CustomerId = customerId,
            ShippingStreet = "123 Main St",
            ShippingCity = "New York",
            ShippingState = "NY",
            ShippingPostalCode = "10001",
            ShippingCountry = "USA",
            OrderItems = new List<CreateOrderItemDto>()
        };

        var orderDto = new OrderDto
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OrderDto>.Success(orderDto));

        // Act
        var result = await _controller.CreateOrder(createDto, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedOrder = Assert.IsType<OrderDto>(createdResult.Value);
        Assert.Equal(orderDto.Id, returnedOrder.Id);
    }

    [Fact]
    public async Task CreateOrder_CustomerNotFound_ReturnsNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var createDto = new CreateOrderDto
        {
            CustomerId = customerId,
            ShippingStreet = "123 Main St",
            ShippingCity = "New York",
            ShippingState = "NY",
            ShippingPostalCode = "10001",
            ShippingCountry = "USA",
            OrderItems = new List<CreateOrderItemDto>()
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OrderDto>.Failure("Customer not found", "CUSTOMER_NOT_FOUND"));

        // Act
        var result = await _controller.CreateOrder(createDto, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateOrder_ProductNotFound_ReturnsNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var createDto = new CreateOrderDto
        {
            CustomerId = customerId,
            ShippingStreet = "123 Main St",
            ShippingCity = "New York",
            ShippingState = "NY",
            ShippingPostalCode = "10001",
            ShippingCountry = "USA",
            OrderItems = new List<CreateOrderItemDto>
            {
                new CreateOrderItemDto { ProductId = productId, Quantity = 2 }
            }
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OrderDto>.Failure("Product not found", "PRODUCT_NOT_FOUND"));

        // Act
        var result = await _controller.CreateOrder(createDto, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateOrder_BusinessRuleViolation_ReturnsUnprocessableEntity()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var createDto = new CreateOrderDto
        {
            CustomerId = customerId,
            ShippingStreet = "123 Main St",
            ShippingCity = "New York",
            ShippingState = "NY",
            ShippingPostalCode = "10001",
            ShippingCountry = "USA",
            OrderItems = new List<CreateOrderItemDto>()
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OrderDto>.Failure("Business rule violation", "BUSINESS_RULE_VIOLATION"));

        // Act
        var result = await _controller.CreateOrder(createDto, CancellationToken.None);

        // Assert
        Assert.IsType<UnprocessableEntityObjectResult>(result);
    }
}
