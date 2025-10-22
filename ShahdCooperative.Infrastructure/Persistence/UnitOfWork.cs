using System.Data;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly DapperContext _context;
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(DapperContext context,
        IProductRepository products, IOrderRepository orders,
        ICustomerRepository customers, IFeedbackRepository feedbacks)
    {
        _context = context;
        Products = products;
        Orders = orders;
        Customers = customers;
        Feedbacks = feedbacks;
    }

    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    public ICustomerRepository Customers { get; }
    public IFeedbackRepository Feedbacks { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // With Dapper, changes are committed immediately on ExecuteAsync
        // This method is kept for interface compatibility
        // Actual transaction control is done via BeginTransaction/Commit/Rollback
        return Task.FromResult(0);
    }

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_connection == null)
        {
            _connection = _context.CreateConnection();
            _connection.Open();
        }

        _transaction = _connection.BeginTransaction();
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }

        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        if (_connection != null)
        {
            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _connection?.Dispose();
            _disposed = true;
        }
    }
}
