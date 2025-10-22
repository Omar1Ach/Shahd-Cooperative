using Dapper;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DapperContext _context;

    public ProductRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, Name, Description, SKU, Category, Type, Price, Currency,
                StockQuantity, ThresholdLevel, ImageUrl, IsActive, CreatedAt, UpdatedAt
            FROM [Inventory].[Products]
            WHERE Id = @Id AND IsActive = 1";

        return await connection.QueryFirstOrDefaultAsync<Product>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, Name, Description, SKU, Category, Type, Price, Currency,
                StockQuantity, ThresholdLevel, ImageUrl, IsActive, CreatedAt, UpdatedAt
            FROM [Inventory].[Products]
            WHERE IsActive = 1
            ORDER BY Name";

        return await connection.QueryAsync<Product>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<Product> AddAsync(Product entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            INSERT INTO [Inventory].[Products]
            (Id, Name, Description, SKU, Category, Type, Price, Currency,
             StockQuantity, ThresholdLevel, ImageUrl, IsActive, CreatedAt, UpdatedAt)
            VALUES
            (@Id, @Name, @Description, @SKU, @Category, @Type, @Price, @Currency,
             @StockQuantity, @ThresholdLevel, @ImageUrl, @IsActive, @CreatedAt, @UpdatedAt)";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));

        return entity;
    }

    public async Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            UPDATE [Inventory].[Products]
            SET Name = @Name,
                Description = @Description,
                Category = @Category,
                Price = @Price,
                Currency = @Currency,
                StockQuantity = @StockQuantity,
                ThresholdLevel = @ThresholdLevel,
                ImageUrl = @ImageUrl,
                IsActive = @IsActive,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(Product entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        // Soft delete
        const string sql = @"
            UPDATE [Inventory].[Products]
            SET IsActive = 0,
                UpdatedAt = GETUTCDATE()
            WHERE Id = @Id";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { entity.Id }, cancellationToken: cancellationToken));
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT CAST(CASE WHEN EXISTS(
                SELECT 1 FROM [Inventory].[Products] WHERE Id = @Id AND IsActive = 1
            ) THEN 1 ELSE 0 END AS BIT)";

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(
        int threshold,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, Name, Description, SKU, Category, Type, Price, Currency,
                StockQuantity, ThresholdLevel, ImageUrl, IsActive, CreatedAt, UpdatedAt
            FROM [Inventory].[Products]
            WHERE StockQuantity <= @Threshold
              AND IsActive = 1
            ORDER BY StockQuantity ASC";

        return await connection.QueryAsync<Product>(
            new CommandDefinition(sql, new { Threshold = threshold }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(
        string category,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, Name, Description, SKU, Category, Type, Price, Currency,
                StockQuantity, ThresholdLevel, ImageUrl, IsActive, CreatedAt, UpdatedAt
            FROM [Inventory].[Products]
            WHERE Category = @Category
              AND IsActive = 1
            ORDER BY Name";

        return await connection.QueryAsync<Product>(
            new CommandDefinition(sql, new { Category = category }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Product>> GetByTypeAsync(
        ProductType type,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, Name, Description, SKU, Category, Type, Price, Currency,
                StockQuantity, ThresholdLevel, ImageUrl, IsActive, CreatedAt, UpdatedAt
            FROM [Inventory].[Products]
            WHERE Type = @Type
              AND IsActive = 1
            ORDER BY Name";

        return await connection.QueryAsync<Product>(
            new CommandDefinition(sql, new { Type = type.ToString() }, cancellationToken: cancellationToken));
    }
}
