using Microsoft.EntityFrameworkCore;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Product>> GetByTypeAsync(
        ProductType type, CancellationToken cancellationToken = default)
    {
        // Convert enum to string for comparison with database NVARCHAR column
        var typeString = type.ToString();
        return await _dbSet.Where(p => p.Type == typeString).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(
        string category, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.Category == category).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(
        int threshold, CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(p => p.StockQuantity <= threshold)
            .OrderBy(p => p.StockQuantity).ToListAsync(cancellationToken);
    }
}
