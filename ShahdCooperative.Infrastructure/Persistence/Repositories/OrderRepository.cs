using Dapper;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Enums;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly DapperContext _context;

    public OrderRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, OrderNumber, CustomerId, OrderDate, Status, TotalAmount, Currency,
                ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry,
                TrackingNumber, CreatedAt, UpdatedAt
            FROM [Sales].[Orders]
            WHERE Id = @Id";

        return await connection.QueryFirstOrDefaultAsync<Order>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, OrderNumber, CustomerId, OrderDate, Status, TotalAmount, Currency,
                ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry,
                TrackingNumber, CreatedAt, UpdatedAt
            FROM [Sales].[Orders]
            ORDER BY OrderDate DESC";

        return await connection.QueryAsync<Order>(
            new CommandDefinition(sql, cancellationToken: cancellationToken));
    }

    public async Task<Order> AddAsync(Order entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            INSERT INTO [Sales].[Orders]
            (Id, OrderNumber, CustomerId, OrderDate, Status, TotalAmount, Currency,
             ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry,
             TrackingNumber, CreatedAt, UpdatedAt)
            VALUES
            (@Id, @OrderNumber, @CustomerId, @OrderDate, @Status, @TotalAmount, @Currency,
             @ShippingStreet, @ShippingCity, @ShippingState, @ShippingPostalCode, @ShippingCountry,
             @TrackingNumber, @CreatedAt, @UpdatedAt)";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));

        return entity;
    }

    public async Task UpdateAsync(Order entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            UPDATE [Sales].[Orders]
            SET Status = @Status,
                TotalAmount = @TotalAmount,
                TrackingNumber = @TrackingNumber,
                UpdatedAt = @UpdatedAt
            WHERE Id = @Id";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, entity, cancellationToken: cancellationToken));
    }

    public async Task DeleteAsync(Order entity, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            DELETE FROM [Sales].[Orders]
            WHERE Id = @Id";

        await connection.ExecuteAsync(
            new CommandDefinition(sql, new { entity.Id }, cancellationToken: cancellationToken));
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT CAST(CASE WHEN EXISTS(
                SELECT 1 FROM [Sales].[Orders] WHERE Id = @Id
            ) THEN 1 ELSE 0 END AS BIT)";

        return await connection.ExecuteScalarAsync<bool>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Order>> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, OrderNumber, CustomerId, OrderDate, Status, TotalAmount, Currency,
                ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry,
                TrackingNumber, CreatedAt, UpdatedAt
            FROM [Sales].[Orders]
            WHERE CustomerId = @CustomerId
            ORDER BY OrderDate DESC";

        return await connection.QueryAsync<Order>(
            new CommandDefinition(sql, new { CustomerId = customerId }, cancellationToken: cancellationToken));
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(
        OrderStatus status,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                Id, OrderNumber, CustomerId, OrderDate, Status, TotalAmount, Currency,
                ShippingStreet, ShippingCity, ShippingState, ShippingPostalCode, ShippingCountry,
                TrackingNumber, CreatedAt, UpdatedAt
            FROM [Sales].[Orders]
            WHERE Status = @Status
            ORDER BY OrderDate DESC";

        return await connection.QueryAsync<Order>(
            new CommandDefinition(sql, new { Status = status.ToString() }, cancellationToken: cancellationToken));
    }

    public async Task<Order?> GetByIdWithItemsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        using var connection = _context.CreateConnection();

        const string sql = @"
            SELECT
                o.Id, o.OrderNumber, o.CustomerId, o.OrderDate, o.Status, o.TotalAmount, o.Currency,
                o.ShippingStreet, o.ShippingCity, o.ShippingState, o.ShippingPostalCode, o.ShippingCountry,
                o.TrackingNumber, o.CreatedAt, o.UpdatedAt,
                oi.Id, oi.OrderId, oi.ProductId, oi.Quantity, oi.UnitPrice, oi.Currency, oi.Discount, oi.Subtotal,
                oi.CreatedAt, oi.UpdatedAt
            FROM [Sales].[Orders] o
            LEFT JOIN [Sales].[OrderItems] oi ON o.Id = oi.OrderId
            WHERE o.Id = @Id";

        var orderDictionary = new Dictionary<Guid, Order>();

        var result = await connection.QueryAsync<Order, OrderItem, Order>(
            new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken),
            (order, orderItem) =>
            {
                if (!orderDictionary.TryGetValue(order.Id, out var orderEntry))
                {
                    orderEntry = order;
                    orderEntry.OrderItems = new List<OrderItem>();
                    orderDictionary.Add(orderEntry.Id, orderEntry);
                }

                if (orderItem != null)
                {
                    ((List<OrderItem>)orderEntry.OrderItems).Add(orderItem);
                }

                return orderEntry;
            },
            splitOn: "Id");

        return orderDictionary.Values.FirstOrDefault();
    }
}
