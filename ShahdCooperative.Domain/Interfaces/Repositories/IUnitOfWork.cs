﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    ICustomerRepository Customers { get; }
    IFeedbackRepository Feedbacks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
