using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        IMapper mapper)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Validate customer exists
        var customer = await _customerRepository.GetByIdAsync(request.Order.CustomerId, cancellationToken);
        if (customer == null)
            return Result<OrderDto>.Failure("Customer not found", "CUSTOMER_NOT_FOUND");

        // Validate all products exist and calculate totals
        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var itemDto in request.Order.OrderItems)
        {
            var product = await _productRepository.GetByIdAsync(itemDto.ProductId, cancellationToken);
            if (product == null)
                return Result<OrderDto>.Failure($"Product {itemDto.ProductId} not found", "PRODUCT_NOT_FOUND");

            if (product.StockQuantity < itemDto.Quantity)
                return Result<OrderDto>.Failure($"Insufficient stock for product {product.Name}", "INSUFFICIENT_STOCK");

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = itemDto.ProductId,
                Quantity = itemDto.Quantity,
                UnitPrice = product.Price,
                Currency = product.Currency,
                Discount = 0,
                Subtotal = product.Price * itemDto.Quantity,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            orderItems.Add(orderItem);
            totalAmount += orderItem.Subtotal;
        }

        // Create order
        var order = _mapper.Map<Order>(request.Order);
        order.Id = Guid.NewGuid();
        order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        order.TotalAmount = totalAmount;
        order.CreatedAt = DateTime.UtcNow;
        order.UpdatedAt = DateTime.UtcNow;

        var createdOrder = await _orderRepository.AddAsync(order, cancellationToken);

        // Add order items
        foreach (var item in orderItems)
        {
            item.OrderId = createdOrder.Id;
            // Note: In a real application, you'd have an OrderItemRepository
            // For now, this assumes the repository handles cascading inserts
        }

        var orderDto = _mapper.Map<OrderDto>(createdOrder);
        orderDto.OrderItems = _mapper.Map<List<OrderItemDto>>(orderItems);

        return Result<OrderDto>.Success(orderDto);
    }
}
