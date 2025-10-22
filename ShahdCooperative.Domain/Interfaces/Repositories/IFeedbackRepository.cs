using ShahdCooperative.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Repositories;

public interface IFeedbackRepository : IRepository<Feedback>
{
    Task<IEnumerable<Feedback>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Feedback>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
}
