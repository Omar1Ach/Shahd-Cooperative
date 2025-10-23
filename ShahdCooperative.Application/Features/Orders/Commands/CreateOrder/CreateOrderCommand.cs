using MediatR;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(CreateOrderDto Order) : IRequest<Result<OrderDto>>;
