using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Application.Features.Customers.Commands.CreateCustomer;
using ShahdCooperative.Application.Features.Customers.Commands.DeleteCustomer;
using ShahdCooperative.Application.Features.Customers.Commands.UpdateCustomer;
using ShahdCooperative.Application.Features.Customers.Queries.GetCustomerById;
using ShahdCooperative.Application.Features.Customers.Queries.GetCustomers;

namespace ShahdCooperative.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(IMediator mediator, ILogger<CustomersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all customers");

        var query = new GetCustomersQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetCustomer(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting customer {CustomerId}", id);

        var query = new GetCustomerByIdQuery(id);
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
    public async Task<IActionResult> CreateCustomer(
        [FromBody] CreateCustomerDto dto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new customer: {CustomerName}", dto.Name);

        var command = new CreateCustomerCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });

        return CreatedAtAction(
            nameof(GetCustomer),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCustomer(
        Guid id,
        [FromBody] UpdateCustomerDto dto,
        CancellationToken cancellationToken)
    {
        if (id != dto.Id)
            return BadRequest(new { error = "Customer ID mismatch", errorCode = "ID_MISMATCH" });

        _logger.LogInformation("Updating customer {CustomerId}", id);

        var command = new UpdateCustomerCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "NOT_FOUND")
                return NotFound(new { error = result.Error, errorCode = result.ErrorCode });

            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCustomer(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting customer {CustomerId}", id);

        var command = new DeleteCustomerCommand(id);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "NOT_FOUND")
                return NotFound(new { error = result.Error, errorCode = result.ErrorCode });

            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });
        }

        return NoContent();
    }
}
