using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Domain.Services;

public interface IOrderDomainService
{
    bool CanCancelOrder(Order order);
    int CalculateLoyaltyPoints(Order order);
}
