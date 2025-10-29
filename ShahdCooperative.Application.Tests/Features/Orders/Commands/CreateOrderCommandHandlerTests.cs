using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Application.Events;
using ShahdCooperative.Application.Features.Orders.Commands.CreateOrder;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Interfaces;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Orders.Commands;

public class CreateOrderCommandHandlerTests
{
    private readonly Mock<IOrderRepository> _mockOrderRepository;
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IEventPublisher> _mockEventPublisher;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _mockOrderRepository = new Mock<IOrderRepository>();
        _mockProductRepository = new Mock<IProductRepository>();
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockEventPublisher = new Mock<IEventPublisher>();
        _handler = new CreateOrderCommandHandler(
            _mockOrderRepository.Object,
            _mockProductRepository.Object,
            _mockCustomerRepository.Object,
            _mockMapper.Object,
            _mockEventPublisher.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var dto = new CreateOrderDto
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

        var command = new CreateOrderCommand(dto);
        var customer = Customer.Create("auth123", "John Doe", "john@example.com");
        var product = Product.Create(
            name: "Test Honey",
            sku: "HON-001",
            category: "Honey",
            type: ProductType.Honey,
            price: 19.99m,
            currency: "USD",
            stockQuantity: 100,
            thresholdLevel: 10);
        var order = Order.Create(customerId, "USD", "123 Main St", "New York", "NY", "10001", "USA");
        var orderDto = new OrderDto { Id = Guid.NewGuid(), CustomerId = customerId };

        _mockCustomerRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _mockOrderRepository.Setup(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order o, CancellationToken ct) => o);
        _mockMapper.Setup(x => x.Map<OrderDto>(It.IsAny<Order>()))
            .Returns(orderDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        _mockOrderRepository.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockEventPublisher.Verify(x => x.PublishAsync("order.created", It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ReturnsFailureResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var dto = new CreateOrderDto
        {
            CustomerId = customerId,
            ShippingStreet = "123 Main St",
            ShippingCity = "New York",
            ShippingState = "NY",
            ShippingPostalCode = "10001",
            ShippingCountry = "USA",
            OrderItems = new List<CreateOrderItemDto>()
        };

        var command = new CreateOrderCommand(dto);

        _mockCustomerRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("CUSTOMER_NOT_FOUND", result.ErrorCode);
        _mockOrderRepository.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ProductNotFound_ReturnsFailureResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var dto = new CreateOrderDto
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

        var command = new CreateOrderCommand(dto);
        var customer = Customer.Create("auth123", "John Doe", "john@example.com");

        _mockCustomerRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockProductRepository.Setup(x => x.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("PRODUCT_NOT_FOUND", result.ErrorCode);
        _mockOrderRepository.Verify(x => x.AddAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
