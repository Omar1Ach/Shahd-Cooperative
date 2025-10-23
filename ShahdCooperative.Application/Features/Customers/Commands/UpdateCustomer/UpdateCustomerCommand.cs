using MediatR;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(UpdateCustomerDto Customer) : IRequest<Result<CustomerDto>>;
