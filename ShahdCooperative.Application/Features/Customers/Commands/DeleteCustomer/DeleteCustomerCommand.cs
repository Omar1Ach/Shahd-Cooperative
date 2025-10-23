using MediatR;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Customers.Commands.DeleteCustomer;

public record DeleteCustomerCommand(Guid Id) : IRequest<Result<bool>>;
