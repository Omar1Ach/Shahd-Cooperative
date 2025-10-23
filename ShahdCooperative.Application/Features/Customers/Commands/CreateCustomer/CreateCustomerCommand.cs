using MediatR;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(CreateCustomerDto Customer) : IRequest<Result<CustomerDto>>;
