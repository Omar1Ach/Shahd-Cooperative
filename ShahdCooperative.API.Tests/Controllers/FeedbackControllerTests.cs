using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ShahdCooperative.API.Controllers;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Application.Features.Feedback.Commands.CreateFeedback;
using ShahdCooperative.Application.Features.Feedback.Commands.DeleteFeedback;
using ShahdCooperative.Application.Features.Feedback.Queries.GetFeedback;
using ShahdCooperative.Application.Features.Feedback.Queries.GetFeedbackById;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.API.Tests.Controllers;

public class FeedbackControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<FeedbackController>> _mockLogger;
    private readonly FeedbackController _controller;

    public FeedbackControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<FeedbackController>>();
        _controller = new FeedbackController(_mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetFeedback_ReturnsOkResult_WithFeedback()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var feedbacks = new List<FeedbackDto>
        {
            new FeedbackDto { Id = Guid.NewGuid(), CustomerId = customerId, Content = "Great!", Rating = 5 },
            new FeedbackDto { Id = Guid.NewGuid(), CustomerId = customerId, Content = "Good", Rating = 4 }
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<GetFeedbackQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IEnumerable<FeedbackDto>>.Success(feedbacks));

        // Act
        var result = await _controller.GetFeedback(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedFeedbacks = Assert.IsAssignableFrom<IEnumerable<FeedbackDto>>(okResult.Value);
        Assert.Equal(2, returnedFeedbacks.Count());
    }

    [Fact]
    public async Task GetFeedbackById_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var feedbackDto = new FeedbackDto
        {
            Id = feedbackId,
            CustomerId = customerId,
            Content = "Great product!",
            Rating = 5
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<GetFeedbackByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<FeedbackDto>.Success(feedbackDto));

        // Act
        var result = await _controller.GetFeedbackById(feedbackId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedFeedback = Assert.IsType<FeedbackDto>(okResult.Value);
        Assert.Equal(feedbackId, returnedFeedback.Id);
    }

    [Fact]
    public async Task GetFeedbackById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();

        _mockMediator.Setup(x => x.Send(It.IsAny<GetFeedbackByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<FeedbackDto>.Failure("Feedback not found", "NOT_FOUND"));

        // Act
        var result = await _controller.GetFeedbackById(feedbackId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateFeedback_ValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var createDto = new CreateFeedbackDto
        {
            CustomerId = customerId,
            Content = "Great product!",
            Rating = 5
        };

        var feedbackDto = new FeedbackDto
        {
            Id = Guid.NewGuid(),
            CustomerId = customerId,
            Content = createDto.Content,
            Rating = createDto.Rating
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateFeedbackCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<FeedbackDto>.Success(feedbackDto));

        // Act
        var result = await _controller.CreateFeedback(createDto, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedFeedback = Assert.IsType<FeedbackDto>(createdResult.Value);
        Assert.Equal(feedbackDto.Id, returnedFeedback.Id);
    }

    [Fact]
    public async Task CreateFeedback_CustomerNotFound_ReturnsNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var createDto = new CreateFeedbackDto
        {
            CustomerId = customerId,
            Content = "Great product!",
            Rating = 5
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateFeedbackCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<FeedbackDto>.Failure("Customer not found", "CUSTOMER_NOT_FOUND"));

        // Act
        var result = await _controller.CreateFeedback(createDto, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteFeedback_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();

        _mockMediator.Setup(x => x.Send(It.IsAny<DeleteFeedbackCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.DeleteFeedback(feedbackId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteFeedback_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();

        _mockMediator.Setup(x => x.Send(It.IsAny<DeleteFeedbackCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Feedback not found", "NOT_FOUND"));

        // Act
        var result = await _controller.DeleteFeedback(feedbackId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
