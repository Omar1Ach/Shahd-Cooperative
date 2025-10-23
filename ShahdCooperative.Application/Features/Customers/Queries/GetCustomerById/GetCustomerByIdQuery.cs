using MediatR;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Customers.Queries.GetCustomerById;

public record GetCustomerByIdQuery(Guid Id) : IRequest<Result<CustomerDto>>;
