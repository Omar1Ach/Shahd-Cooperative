using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Domain.Services;

/// <summary>
/// Domain service for pricing calculations and discount logic
/// </summary>
public class PricingDomainService : IPricingDomainService
{
    private const decimal MaxDiscountPercent = 100m;
    private const decimal MinDiscountPercent = 0m;

    /// <summary>
    /// Calculates the total amount for a collection of order items
    /// </summary>
    public decimal CalculateOrderTotal(IEnumerable<OrderItem> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        return items.Sum(item => item.Subtotal);
    }

    /// <summary>
    /// Applies a percentage discount to an original price
    /// </summary>
    /// <param name="originalPrice">The original price before discount</param>
    /// <param name="discountPercent">The discount percentage (0-100)</param>
    /// <returns>The discounted price</returns>
    public decimal ApplyDiscount(decimal originalPrice, decimal discountPercent)
    {
        if (originalPrice < 0)
            throw new ArgumentException("Original price cannot be negative", nameof(originalPrice));

        if (discountPercent < MinDiscountPercent || discountPercent > MaxDiscountPercent)
            throw new ArgumentException(
                $"Discount percent must be between {MinDiscountPercent} and {MaxDiscountPercent}",
                nameof(discountPercent));

        var discountAmount = originalPrice * (discountPercent / 100m);
        var discountedPrice = originalPrice - discountAmount;

        // Ensure price doesn't go below zero
        return Math.Max(discountedPrice, 0m);
    }
}
