using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Application.Events;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;
    private readonly IEventPublisher _eventPublisher;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IMapper mapper,
        IEventPublisher eventPublisher)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate customer exists
        var customer = await _customerRepository.GetByIdAsync(request.Order.CustomerId, cancellationToken);
        if (customer == null)
            return Result<OrderDto>.Failure("Customer not found", "CUSTOMER_NOT_FOUND");

        // Create order using factory method
        var order = Order.Create(
            request.Order.CustomerId,
            "USD",
            request.Order.ShippingStreet,
            request.Order.ShippingCity,
            request.Order.ShippingState,
            request.Order.ShippingPostalCode,
            request.Order.ShippingCountry);

        // Add items to order
        foreach (var itemDto in request.Order.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId, cancellationToken);
            if (product == null)
                return Result<OrderDto>.Failure($"Product {itemDto.ProductId} not found", "PRODUCT_NOT_FOUND");

            try
            {
                order.AddItem(product, itemDto.Quantity, product.Price);
            }
            catch (InvalidOperationException ex)
            {
                return Result<OrderDto>.Failure(ex.Message, "BUSINESS_RULE_VIOLATION");
            }
        }

        var createdOrder = await _orderRepository.AddAsync(order, cancellationToken);

        var orderDto = _mapper.Map<OrderDto>(createdOrder);

        // Publish OrderCreatedEvent to RabbitMQ for NotificationService
        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = createdOrder.Id,
            CustomerId = customer.Id,
            CustomerEmail = customer.Email,
            CustomerName = customer.Name,
            TotalAmount = createdOrder.TotalAmount,
            Currency = "USD",
            ItemCount = createdOrder.OrderItems.Count,
            ShippingAddress = $"{createdOrder.ShippingStreet}, {createdOrder.ShippingCity}, {createdOrder.ShippingState} {createdOrder.ShippingPostalCode}, {createdOrder.ShippingCountry}",
            CreatedAt = createdOrder.CreatedAt
        };

        await _eventPublisher.PublishAsync("order.created", orderCreatedEvent, cancellationToken);

        return Result<OrderDto>.Success(orderDto);
    }
}
