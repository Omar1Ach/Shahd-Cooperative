using Microsoft.EntityFrameworkCore;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class FeedbackRepository : Repository<Feedback>, IFeedbackRepository
{
    public FeedbackRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Feedback>> GetByProductIdAsync(
        Guid productId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(f => f.ProductId == productId)
            .Include(f => f.Customer).OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Feedback>> GetByCustomerIdAsync(
        Guid customerId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(f => f.CustomerId == customerId)
            .Include(f => f.Product).OrderByDescending(f => f.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
