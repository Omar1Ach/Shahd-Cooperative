using MediatR;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Customers.Queries.GetCustomers;

public record GetCustomersQuery : IRequest<Result<IEnumerable<CustomerDto>>>;
