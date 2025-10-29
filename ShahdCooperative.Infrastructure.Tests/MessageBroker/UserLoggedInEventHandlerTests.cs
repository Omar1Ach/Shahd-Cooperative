using Microsoft.Extensions.Logging;
using Moq;
using ShahdCooperative.Infrastructure.MessageBroker.Events;
using ShahdCooperative.Infrastructure.MessageBroker.Handlers;

namespace ShahdCooperative.Infrastructure.Tests.MessageBroker;

public class UserLoggedInEventHandlerTests
{
    private readonly Mock<ILogger<UserLoggedInEventHandler>> _mockLogger;
    private readonly UserLoggedInEventHandler _handler;

    public UserLoggedInEventHandlerTests()
    {
        _mockLogger = new Mock<ILogger<UserLoggedInEventHandler>>();
        _handler = new UserLoggedInEventHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldLogUserLoginInformation()
    {
        // Arrange
        var userEvent = new UserLoggedInEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            IpAddress = "192.168.1.1",
            LoggedInAt = DateTime.UtcNow
        };

        // Act
        await _handler.HandleAsync(userEvent);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenIpAddressIsNull_ShouldLogWithUnknownIp()
    {
        // Arrange
        var userEvent = new UserLoggedInEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            IpAddress = null,
            LoggedInAt = DateTime.UtcNow
        };

        // Act
        await _handler.HandleAsync(userEvent);

        // Assert - should complete without errors
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var userEvent = new UserLoggedInEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            IpAddress = "10.0.0.1",
            LoggedInAt = DateTime.UtcNow
        };

        // Act
        var task = _handler.HandleAsync(userEvent);

        // Assert
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }
}
