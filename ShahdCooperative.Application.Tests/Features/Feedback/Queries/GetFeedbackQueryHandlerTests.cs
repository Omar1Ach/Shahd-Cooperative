using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Application.Features.Feedback.Queries.GetFeedback;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Feedback.Queries;

public class GetFeedbackQueryHandlerTests
{
    private readonly Mock<IFeedbackRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetFeedbackQueryHandler _handler;

    public GetFeedbackQueryHandlerTests()
    {
        _mockRepository = new Mock<IFeedbackRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetFeedbackQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllFeedback()
    {
        // Arrange
        var query = new GetFeedbackQuery();
        var customerId = Guid.NewGuid();
        var feedbacks = new List<Domain.Entities.Feedback>
        {
            Domain.Entities.Feedback.Create(customerId, "Great product!", 5),
            Domain.Entities.Feedback.Create(customerId, "Good service", 4)
        };

        var feedbackDtos = new List<FeedbackDto>
        {
            new FeedbackDto { Id = Guid.NewGuid(), CustomerId = customerId, Content = "Great product!", Rating = 5 },
            new FeedbackDto { Id = Guid.NewGuid(), CustomerId = customerId, Content = "Good service", Rating = 4 }
        };

        _mockRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedbacks);
        _mockMapper.Setup(x => x.Map<IEnumerable<FeedbackDto>>(It.IsAny<IEnumerable<Domain.Entities.Feedback>>()))
            .Returns(feedbackDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
        _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoFeedback_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetFeedbackQuery();
        var feedbacks = new List<Domain.Entities.Feedback>();
        var feedbackDtos = new List<FeedbackDto>();

        _mockRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedbacks);
        _mockMapper.Setup(x => x.Map<IEnumerable<FeedbackDto>>(It.IsAny<IEnumerable<Domain.Entities.Feedback>>()))
            .Returns(feedbackDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }
}
