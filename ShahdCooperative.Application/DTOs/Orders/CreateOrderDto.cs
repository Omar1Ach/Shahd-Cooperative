namespace ShahdCooperative.Application.DTOs.Orders;

public class CreateOrderDto
{
    public Guid CustomerId { get; set; }
    public string? ShippingStreet { get; set; }
    public string? ShippingCity { get; set; }
    public string? ShippingState { get; set; }
    public string? ShippingPostalCode { get; set; }
    public string? ShippingCountry { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; } = new();
}

public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
