using MediatR;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, Result<bool>>
{
    private readonly ICustomerRepository _customerRepository;

    public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<bool>> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);

        if (customer == null)
            return Result<bool>.Failure("Customer not found", "NOT_FOUND");

        await _customerRepository.DeleteAsync(customer, cancellationToken);
        return Result<bool>.Success(true);
    }
}
