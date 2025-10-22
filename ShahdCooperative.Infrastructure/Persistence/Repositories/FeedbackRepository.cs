using Dapper;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly DapperContext _context;

    public FeedbackRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Feedback?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, CustomerId, ProductId, OrderId, Content, Rating, Response,
                RespondedBy, RespondedAt, Status, CreatedAt, UpdatedAt
            FROM [Core].[Feedback]
            WHERE Id = @Id";

        return await connection.QueryFirstOrDefaultAsync<Feedback>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Feedback>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, CustomerId, ProductId, OrderId, Content, Rating, Response,
                RespondedBy, RespondedAt, Status, CreatedAt, UpdatedAt
            FROM [Core].[Feedback]
            ORDER BY CreatedAt DESC";

        return await connection.QueryAsync<Feedback>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<Feedback> AddAsync(Feedback entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            INSERT INTO [Core].[Feedback]
            (Id, CustomerId, ProductId, OrderId, Content, Rating, Response,
             RespondedBy, RespondedAt, Status, CreatedAt, UpdatedAt)
            VALUES
            (@Id, @CustomerId, @ProductId, @OrderId, @Content, @Rating, @Response,
             @RespondedBy, @RespondedAt, @Status, @CreatedAt, @UpdatedAt)";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));

        return entity;
    }

    public async Task UpdateAsync(Feedback entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            UPDATE [Core].[Feedback]
            SET Content = @Content,
                Rating = @Rating,
                Response = @Response,
                RespondedBy = @RespondedBy,
                RespondedAt = @RespondedAt,
                Status = @Status,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(Feedback entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            DELETE FROM [Core].[Feedback]
            WHERE Id = @Id";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { entity.Id }, cancellationToken: cancellationToken));
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT CAST(CASE WHEN EXISTS(
                SELECT 1 FROM [Core].[Feedback] WHERE Id = @Id
            ) THEN 1 ELSE 0 END AS BIT)";

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Feedback>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, CustomerId, ProductId, OrderId, Content, Rating, Response,
                RespondedBy, RespondedAt, Status, CreatedAt, UpdatedAt
            FROM [Core].[Feedback]
            WHERE ProductId = @ProductId
            ORDER BY CreatedAt DESC";

        return await connection.QueryAsync<Feedback>(
            new CommandDefinition(sql, new { ProductId = productId }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Feedback>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, CustomerId, ProductId, OrderId, Content, Rating, Response,
                RespondedBy, RespondedAt, Status, CreatedAt, UpdatedAt
            FROM [Core].[Feedback]
            WHERE CustomerId = @CustomerId
            ORDER BY CreatedAt DESC";

        return await connection.QueryAsync<Feedback>(
            new CommandDefinition(sql, new { CustomerId = customerId }, cancellationToken: cancellationToken));
    }
}
