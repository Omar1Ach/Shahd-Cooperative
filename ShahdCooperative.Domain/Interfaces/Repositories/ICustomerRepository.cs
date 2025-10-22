using ShahdCooperative.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ShahdCooperative.Domain.Interfaces.Repositories;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<Customer?> GetByAuthIdAsync(string authId, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
