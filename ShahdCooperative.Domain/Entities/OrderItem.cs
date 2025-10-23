namespace ShahdCooperative.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string Currency { get; private set; } = "USD";
    public decimal Discount { get; private set; } = 0;
    public decimal Subtotal { get; private set; }

    // Navigation properties
    public Order Order { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    // Parameterless constructor for EF Core
    private OrderItem() { }

    // Factory method
    public static OrderItem Create(Guid orderId, Guid productId, int quantity, decimal unitPrice, string currency, decimal discount = 0)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Order ID cannot be empty", nameof(orderId));

        if (productId == Guid.Empty)
            throw new ArgumentException("Product ID cannot be empty", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        if (discount < 0 || discount > unitPrice * quantity)
            throw new ArgumentException("Invalid discount amount", nameof(discount));

        var orderItem = new OrderItem
        {
            OrderId = orderId,
            ProductId = productId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Currency = currency,
            Discount = discount
        };

        orderItem.CalculateSubtotal();
        return orderItem;
    }

    // Business methods
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(newQuantity));

        Quantity = newQuantity;
        CalculateSubtotal();
    }

    public void ApplyDiscount(decimal discount)
    {
        if (discount < 0 || discount > UnitPrice * Quantity)
            throw new ArgumentException("Invalid discount amount", nameof(discount));

        Discount = discount;
        CalculateSubtotal();
    }

    private void CalculateSubtotal()
    {
        Subtotal = (UnitPrice * Quantity) - Discount;
    }
}
