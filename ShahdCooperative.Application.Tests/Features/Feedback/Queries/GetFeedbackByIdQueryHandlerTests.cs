using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Application.Features.Feedback.Queries.GetFeedbackById;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Feedback.Queries;

public class GetFeedbackByIdQueryHandlerTests
{
    private readonly Mock<IFeedbackRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetFeedbackByIdQueryHandler _handler;

    public GetFeedbackByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<IFeedbackRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetFeedbackByIdQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_FeedbackExists_ReturnsSuccessResult()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var query = new GetFeedbackByIdQuery(feedbackId);
        var feedback = Domain.Entities.Feedback.Create(customerId, "Great product!", 5);
        var feedbackDto = new FeedbackDto
        {
            Id = feedbackId,
            CustomerId = customerId,
            Content = "Great product!",
            Rating = 5
        };

        _mockRepository.Setup(x => x.GetByIdAsync(feedbackId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);
        _mockMapper.Setup(x => x.Map<FeedbackDto>(It.IsAny<Domain.Entities.Feedback>()))
            .Returns(feedbackDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(feedbackId, result.Value.Id);
        _mockRepository.Verify(x => x.GetByIdAsync(feedbackId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_FeedbackNotFound_ReturnsFailureResult()
    {
        // Arrange
        var feedbackId = Guid.NewGuid();
        var query = new GetFeedbackByIdQuery(feedbackId);

        _mockRepository.Setup(x => x.GetByIdAsync(feedbackId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.Feedback?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
        Assert.Null(result.Value);
    }
}
