using Microsoft.EntityFrameworkCore;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(o => o.CustomerId == customerId)
            .Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.OrderDate).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(
        OrderStatus status, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(o => o.Status == status)
            .Include(o => o.OrderItems).OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order?> GetByIdWithItemsAsync(
        Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
            .Include(o => o.Customer).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }
}
