using MediatR;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(Guid Id) : IRequest<Result<OrderDto>>;
