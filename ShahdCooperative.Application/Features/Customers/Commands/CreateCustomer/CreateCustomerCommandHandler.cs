using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public CreateCustomerCommandHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var customer = Customer.Create(
                request.Customer.ExternalAuthId,
                request.Customer.Name,
                request.Customer.Email,
                request.Customer.Phone,
                request.Customer.Street,
                request.Customer.City,
                request.Customer.State,
                request.Customer.PostalCode,
                request.Customer.Country);

            var createdCustomer = await _customerRepository.AddAsync(customer, cancellationToken);
            var customerDto = _mapper.Map<CustomerDto>(createdCustomer);

            return Result<CustomerDto>.Success(customerDto);
        }
        catch (ArgumentException ex)
        {
            return Result<CustomerDto>.Failure(ex.Message, "VALIDATION_ERROR");
        }
    }
}
