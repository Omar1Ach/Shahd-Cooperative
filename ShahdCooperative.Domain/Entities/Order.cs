using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Events;

namespace ShahdCooperative.Domain.Entities;

/// <summary>
/// Order Aggregate Root - manages order lifecycle and order items
/// </summary>
public class Order : BaseEntity
{
    private readonly List<OrderItem> _orderItems = new();

    // Private setters for encapsulation
    public string OrderNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public DateTime OrderDate { get; private set; } = DateTime.UtcNow;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; private set; }
    public string Currency { get; private set; } = "USD";
    public string? ShippingStreet { get; private set; }
    public string? ShippingCity { get; private set; }
    public string? ShippingState { get; private set; }
    public string? ShippingPostalCode { get; private set; }
    public string? ShippingCountry { get; private set; }
    public string? TrackingNumber { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = null!;
    public IReadOnlyList<OrderItem> OrderItems => _orderItems.AsReadOnly();

    // Parameterless constructor for EF Core
    private Order() { }

    // Factory method for creating orders
    public static Order Create(
        Guid customerId,
        string currency,
        string? shippingStreet,
        string? shippingCity,
        string? shippingState,
        string? shippingPostalCode,
        string? shippingCountry)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Customer ID cannot be empty", nameof(customerId));

        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            CustomerId = customerId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Currency = currency,
            ShippingStreet = shippingStreet,
            ShippingCity = shippingCity,
            ShippingState = shippingState,
            ShippingPostalCode = shippingPostalCode,
            ShippingCountry = shippingCountry,
            TotalAmount = 0
        };

        return order;
    }

    // Business methods
    public void AddItem(Product product, int quantity, decimal unitPrice)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot add items to an order that is not pending");

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        if (!product.CanFulfillOrder(quantity))
            throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

        var orderItem = OrderItem.Create(Id, product.Id, quantity, unitPrice, Currency);
        _orderItems.Add(orderItem);

        CalculateTotal();
    }

    public void RemoveItem(Guid orderItemId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot remove items from an order that is not pending");

        var item = _orderItems.FirstOrDefault(x => x.Id == orderItemId);
        if (item == null)
            throw new InvalidOperationException("Order item not found");

        _orderItems.Remove(item);
        CalculateTotal();
    }

    public void UpdateItemQuantity(Guid orderItemId, int newQuantity)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Cannot update items in an order that is not pending");

        var item = _orderItems.FirstOrDefault(x => x.Id == orderItemId);
        if (item == null)
            throw new InvalidOperationException("Order item not found");

        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        item.UpdateQuantity(newQuantity);
        CalculateTotal();
    }

    public void PlaceOrder()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Order has already been placed");

        if (!_orderItems.Any())
            throw new InvalidOperationException("Cannot place an order with no items");

        Status = OrderStatus.Processing;
        AddDomainEvent(new OrderPlacedEvent(Id, CustomerId, TotalAmount, OrderDate));
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        if (Status == newStatus)
            return;

        if (!CanTransitionTo(newStatus))
            throw new InvalidOperationException($"Cannot transition from {Status} to {newStatus}");

        var oldStatus = Status;
        Status = newStatus;

        AddDomainEvent(new OrderStatusChangedEvent(Id, oldStatus, newStatus, DateTime.UtcNow));
    }

    public void Cancel()
    {
        if (!CanBeCancelled())
            throw new InvalidOperationException($"Order in {Status} status cannot be cancelled");

        UpdateStatus(OrderStatus.Cancelled);
    }

    public void SetTrackingNumber(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new ArgumentException("Tracking number cannot be empty", nameof(trackingNumber));

        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Can only set tracking number for shipped orders");

        TrackingNumber = trackingNumber;
    }

    public void CalculateTotal()
    {
        TotalAmount = _orderItems.Sum(item => item.Subtotal);
    }

    public bool CanBeCancelled()
    {
        return Status == OrderStatus.Pending || Status == OrderStatus.Processing;
    }

    private bool CanTransitionTo(OrderStatus newStatus)
    {
        return (Status, newStatus) switch
        {
            (OrderStatus.Pending, OrderStatus.Processing) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Processing, OrderStatus.Shipped) => true,
            (OrderStatus.Processing, OrderStatus.Cancelled) => true,
            (OrderStatus.Shipped, OrderStatus.Delivered) => true,
            _ => false
        };
    }

    private static string GenerateOrderNumber()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Random.Shared.Next(0, 99999).ToString("D5");
        return $"ORD-{datePart}-{randomPart}";
    }
}
