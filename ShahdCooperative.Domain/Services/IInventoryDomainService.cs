using ShahdCooperative.Domain.Entities;

namespace ShahdCooperative.Domain.Services;

public interface IInventoryDomainService
{
    bool ShouldTriggerLowStockAlert(Product product);
    int CalculateReorderQuantity(Product product);
}
