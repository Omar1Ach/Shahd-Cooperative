using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public UpdateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Result<CustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Customer.Id, cancellationToken);

        if (customer == null)
            return Result<CustomerDto>.Failure("Customer not found", "NOT_FOUND");

        try
        {
            customer.UpdateProfile(
                request.Customer.Name,
                request.Customer.Email,
                request.Customer.Phone);

            customer.UpdateAddress(
                request.Customer.Street,
                request.Customer.City,
                request.Customer.State,
                request.Customer.PostalCode,
                request.Customer.Country);

            await _customerRepository.UpdateAsync(customer, cancellationToken);
            var customerDto = _mapper.Map<CustomerDto>(customer);

            return Result<CustomerDto>.Success(customerDto);
        }
        catch (ArgumentException ex)
        {
            return Result<CustomerDto>.Failure(ex.Message, "VALIDATION_ERROR");
        }
    }
}
