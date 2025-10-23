using Moq;
using ShahdCooperative.Application.Features.Feedback.Commands.DeleteFeedback;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Feedback.Commands;

public class DeleteFeedbackCommandHandlerTests
{
    private readonly Mock<IFeedbackRepository> _mockRepository;
    private readonly DeleteFeedbackCommandHandler _handler;

    public DeleteFeedbackCommandHandlerTests()
    {
        _mockRepository = new Mock<IFeedbackRepository>();
        _handler = new DeleteFeedbackCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var command = new DeleteFeedbackCommand(feedbackId);
        var feedback = Domain.Entities.Feedback.Create(customerId, "Great product!", 5);

        _mockRepository.Setup(x => x.GetByIdAsync(feedbackId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<Domain.Entities.Feedback>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.Feedback>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FeedbackNotFound_ReturnsFailureResult()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var command = new DeleteFeedbackCommand(feedbackId);

        _mockRepository.Setup(x => x.GetByIdAsync(feedbackId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Feedback?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Domain.Entities.Feedback>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
