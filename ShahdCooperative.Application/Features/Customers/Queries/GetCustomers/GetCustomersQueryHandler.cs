using AutoMapper;
using MediatR;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Domain.Common;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Features.Customers.Queries.GetCustomers;

public class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Result<IEnumerable<CustomerDto>>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IMapper _mapper;

    public GetCustomersQueryHandler(ICustomerRepository customerRepository, IMapper _mapper)
    {
        _customerRepository = customerRepository;
        this._mapper = _mapper;
    }

    public async Task<Result<IEnumerable<CustomerDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var customers = await _customerRepository.GetAllAsync(cancellationToken);
        var customerDtos = _mapper.Map<IEnumerable<CustomerDto>>(customers);
        return Result<IEnumerable<CustomerDto>>.Success(customerDtos);
    }
}
