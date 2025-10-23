using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Domain.Services;

/// <summary>
/// Domain service for inventory-related business logic that doesn't naturally fit within a single entity
/// </summary>
public class InventoryDomainService : IInventoryDomainService
{
    private const int DefaultReorderMultiplier = 2;
    private const int MinimumReorderQuantity = 10;

    /// <summary>
    /// Determines if a low stock alert should be triggered for a product
    /// </summary>
    public bool ShouldTriggerLowStockAlert(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        // Alert if stock is at or below threshold and product is active
        return product.StockQuantity <= product.ThresholdLevel && !product.IsDeleted;
    }

    /// <summary>
    /// Calculates the recommended reorder quantity based on threshold and current stock
    /// </summary>
    public int CalculateReorderQuantity(Product product)
    {
        if (product == null)
            throw new ArgumentNullException(nameof(product));

        // If stock is above threshold, no reorder needed
        if (product.StockQuantity > product.ThresholdLevel)
            return 0;

        // Calculate reorder quantity as the difference to reach 2x threshold level
        var targetStock = product.ThresholdLevel * DefaultReorderMultiplier;
        var reorderQuantity = targetStock - product.StockQuantity;

        // Ensure minimum reorder quantity
        return Math.Max(reorderQuantity, MinimumReorderQuantity);
    }
}
