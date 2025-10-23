using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<Result<CustomerDto>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);

        if (customer == null)
            return Result<CustomerDto>.Failure("Customer not found", "NOT_FOUND");

        var customerDto = _mapper.Map<CustomerDto>(customer);
        return Result<CustomerDto>.Success(customerDto);
    }
}
