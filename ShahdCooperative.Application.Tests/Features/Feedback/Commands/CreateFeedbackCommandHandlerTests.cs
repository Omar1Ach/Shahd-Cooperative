using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Feedback;
using ShahdCooperative.Application.Features.Feedback.Commands.CreateFeedback;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Feedback.Commands;

public class CreateFeedbackCommandHandlerTests
{
    private readonly Mock<IFeedbackRepository> _mockFeedbackRepository;
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CreateFeedbackCommandHandler _handler;

    public CreateFeedbackCommandHandlerTests()
    {
        _mockFeedbackRepository = new Mock<IFeedbackRepository>();
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new CreateFeedbackCommandHandler(
            _mockFeedbackRepository.Object,
            _mockCustomerRepository.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var dto = new CreateFeedbackDto
        {
            CustomerId = customerId,
            Content = "Great product!",
            Rating = 5
        };

        var command = new CreateFeedbackCommand(dto);
        var customer = Customer.Create("auth123", "John Doe", "john@example.com");
        var feedback = Domain.Entities.Feedback.Create(customerId, "Great product!", 5);
        var feedbackDto = new FeedbackDto { Id = Guid.NewGuid(), CustomerId = customerId, Content = "Great product!", Rating = 5 };

        _mockCustomerRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockFeedbackRepository.Setup(x => x.AddAsync(It.IsAny<Domain.Entities.Feedback>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(feedback);
        _mockMapper.Setup(x => x.Map<FeedbackDto>(It.IsAny<Domain.Entities.Feedback>()))
            .Returns(feedbackDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(5, result.Value.Rating);
        _mockFeedbackRepository.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Feedback>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ReturnsFailureResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var dto = new CreateFeedbackDto
        {
            CustomerId = customerId,
            Content = "Great product!",
            Rating = 5
        };

        var command = new CreateFeedbackCommand(dto);

        _mockCustomerRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("CUSTOMER_NOT_FOUND", result.ErrorCode);
        _mockFeedbackRepository.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Feedback>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_InvalidRating_ReturnsFailureResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var dto = new CreateFeedbackDto
        {
            CustomerId = customerId,
            Content = "Great product!",
            Rating = 6  // Invalid rating (must be 1-5)
        };

        var command = new CreateFeedbackCommand(dto);
        var customer = Customer.Create("auth123", "John Doe", "john@example.com");

        _mockCustomerRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VALIDATION_ERROR", result.ErrorCode);
        _mockFeedbackRepository.Verify(x => x.AddAsync(It.IsAny<Domain.Entities.Feedback>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
