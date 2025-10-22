using Dapper;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly DapperContext _context;

    public CustomerRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, ExternalAuthId, Name, Email, Phone, Street, City, State, PostalCode, Country,
                LoyaltyPoints, IsActive, CreatedAt, UpdatedAt
            FROM [Core].[Customers]
            WHERE Id = @Id";

        return await connection.QueryFirstOrDefaultAsync<Customer>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, ExternalAuthId, Name, Email, Phone, Street, City, State, PostalCode, Country,
                LoyaltyPoints, IsActive, CreatedAt, UpdatedAt
            FROM [Core].[Customers]
            WHERE IsActive = 1
            ORDER BY Name";

        return await connection.QueryAsync<Customer>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<Customer> AddAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            INSERT INTO [Core].[Customers]
            (Id, ExternalAuthId, Name, Email, Phone, Street, City, State, PostalCode, Country,
             LoyaltyPoints, IsActive, CreatedAt, UpdatedAt)
            VALUES
            (@Id, @ExternalAuthId, @Name, @Email, @Phone, @Street, @City, @State, @PostalCode, @Country,
             @LoyaltyPoints, @IsActive, @CreatedAt, @UpdatedAt)";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));

        return entity;
    }

    public async Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            UPDATE [Core].[Customers]
            SET Name = @Name,
                Email = @Email,
                Phone = @Phone,
                Street = @Street,
                City = @City,
                State = @State,
                PostalCode = @PostalCode,
                Country = @Country,
                LoyaltyPoints = @LoyaltyPoints,
                IsActive = @IsActive,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            UPDATE [Core].[Customers]
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
                SELECT 1 FROM [Core].[Customers] WHERE Id = @Id
            ) THEN 1 ELSE 0 END AS BIT)";

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<Customer?> GetByAuthIdAsync(
        string authId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, ExternalAuthId, Name, Email, Phone, Street, City, State, PostalCode, Country,
                LoyaltyPoints, IsActive, CreatedAt, UpdatedAt
            FROM [Core].[Customers]
            WHERE ExternalAuthId = @AuthId";

        return await connection.QueryFirstOrDefaultAsync<Customer>(
            new CommandDefinition(sql, new { AuthId = authId }, cancellationToken: cancellationToken));
    }

    public async Task<Customer?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, ExternalAuthId, Name, Email, Phone, Street, City, State, PostalCode, Country,
                LoyaltyPoints, IsActive, CreatedAt, UpdatedAt
            FROM [Core].[Customers]
            WHERE Email = @Email";

        return await connection.QueryFirstOrDefaultAsync<Customer>(
            new CommandDefinition(sql, new { Email = email }, cancellationToken: cancellationToken));
    }
}
