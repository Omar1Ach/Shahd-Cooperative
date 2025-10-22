using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Domain.Services;

public interface IPricingDomainService
{
    decimal CalculateOrderTotal(IEnumerable<OrderItem> items);
    decimal ApplyDiscount(decimal originalPrice, decimal discountPercent);
}
