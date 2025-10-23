using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Application.Features.Orders.Commands.CreateOrder;
using ShahdCooperative.Application.Features.Orders.Queries.GetOrderById;
using ShahdCooperative.Application.Features.Orders.Queries.GetOrders;

namespace ShahdCooperative.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "CustomerOrAdmin")] // Orders require Customer or Admin role
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all orders");

        var query = new GetOrdersQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrder(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting order {OrderId}", id);

        var query = new GetOrderByIdQuery(id);
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "NOT_FOUND")
                return NotFound(new { error = result.Error, errorCode = result.ErrorCode });

            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderDto dto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new order for customer {CustomerId}", dto.CustomerId);

        var command = new CreateOrderCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "CUSTOMER_NOT_FOUND" || result.ErrorCode == "PRODUCT_NOT_FOUND")
                return NotFound(new { error = result.Error, errorCode = result.ErrorCode });

            if (result.ErrorCode == "BUSINESS_RULE_VIOLATION" || result.ErrorCode == "INSUFFICIENT_STOCK")
                return UnprocessableEntity(new { error = result.Error, errorCode = result.ErrorCode });

            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });
        }

        return CreatedAtAction(
            nameof(GetOrder),
            new { id = result.Value!.Id },
            result.Value);
    }
}
