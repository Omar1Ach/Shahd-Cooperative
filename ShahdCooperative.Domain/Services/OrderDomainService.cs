using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;

namespace ShahdCooperative.Domain.Services;

/// <summary>
/// Domain service for order-related business logic that spans multiple entities
/// </summary>
public class OrderDomainService : IOrderDomainService
{
    private const decimal LoyaltyPointsPerCurrency = 0.01m; // 1 point per currency unit
    private const int MinimumPointsToAward = 1;

    /// <summary>
    /// Determines if an order can be cancelled based on its current status
    /// </summary>
    public bool CanCancelOrder(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        // Orders can only be cancelled if they're Pending or Processing
        return order.Status == OrderStatus.Pending ||
               order.Status == OrderStatus.Processing;
    }

    /// <summary>
    /// Calculates loyalty points earned for an order based on total amount
    /// </summary>
    public int CalculateLoyaltyPoints(Order order)
    {
        if (order == null)
            throw new ArgumentNullException(nameof(order));

        // No points for cancelled orders
        if (order.Status == OrderStatus.Cancelled)
            return 0;

        // No points for orders that haven't been delivered yet
        if (order.Status != OrderStatus.Delivered)
            return 0;

        // Calculate points: 1 point per currency unit (e.g., $1 = 1 point)
        var points = (int)Math.Floor(order.TotalAmount * LoyaltyPointsPerCurrency);

        // Ensure minimum points are awarded for completed orders
        return Math.Max(points, MinimumPointsToAward);
    }
}
