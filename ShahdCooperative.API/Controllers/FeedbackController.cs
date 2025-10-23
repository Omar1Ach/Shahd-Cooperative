using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Application.Features.Feedback.Commands.CreateFeedback;
using ShahdCooperative.Application.Features.Feedback.Commands.DeleteFeedback;
using ShahdCooperative.Application.Features.Feedback.Queries.GetFeedback;
using ShahdCooperative.Application.Features.Feedback.Queries.GetFeedbackById;

namespace ShahdCooperative.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FeedbackController> _logger;

    public FeedbackController(IMediator mediator, ILogger<FeedbackController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetFeedback(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all feedback");

        var query = new GetFeedbackQuery();
        var result = await _mediator.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetFeedbackById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting feedback {FeedbackId}", id);

        var query = new GetFeedbackByIdQuery(id);
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
    public async Task<IActionResult> CreateFeedback(
        [FromBody] CreateFeedbackDto dto,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new feedback from customer {CustomerId}", dto.CustomerId);

        var command = new CreateFeedbackCommand(dto);
        var result = await _mediator.Send(command, cancellationToken);

        if (!result.IsSuccess)
        {
            if (result.ErrorCode == "CUSTOMER_NOT_FOUND")
                return NotFound(new { error = result.Error, errorCode = result.ErrorCode });

            return BadRequest(new { error = result.Error, errorCode = result.ErrorCode });
        }

        return CreatedAtAction(
            nameof(GetFeedbackById),
            new { id = result.Value!.Id },
            result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteFeedback(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting feedback {FeedbackId}", id);

        var command = new DeleteFeedbackCommand(id);
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
