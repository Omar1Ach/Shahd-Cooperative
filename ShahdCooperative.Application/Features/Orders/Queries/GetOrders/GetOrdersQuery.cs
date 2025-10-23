using MediatR;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Orders.Queries.GetOrders;

public record GetOrdersQuery : IRequest<Result<IEnumerable<OrderDto>>>;
