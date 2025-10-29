using Microsoft.Extensions.Logging;
using Moq;
using ShahdCooperative.Infrastructure.MessageBroker.Events;
using ShahdCooperative.Infrastructure.MessageBroker.Handlers;

namespace ShahdCooperative.Infrastructure.Tests.MessageBroker;

public class UserLoggedOutEventHandlerTests
{
    private readonly Mock<ILogger<UserLoggedOutEventHandler>> _mockLogger;
    private readonly UserLoggedOutEventHandler _handler;

    public UserLoggedOutEventHandlerTests()
    {
        _mockLogger = new Mock<ILogger<UserLoggedOutEventHandler>>();
        _handler = new UserLoggedOutEventHandler(_mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldLogUserLogoutInformation()
    {
        // Arrange
        var userEvent = new UserLoggedOutEvent
        {
            UserId = Guid.NewGuid(),
            LoggedOutAt = DateTime.UtcNow
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
    public async Task HandleAsync_ShouldCompleteSuccessfully()
    {
        // Arrange
        var userEvent = new UserLoggedOutEvent
        {
            UserId = Guid.NewGuid(),
            LoggedOutAt = DateTime.UtcNow
        };

        // Act
        var task = _handler.HandleAsync(userEvent);

        // Assert
        await task;
        Assert.True(task.IsCompletedSuccessfully);
    }

    [Fact]
    public async Task HandleAsync_WithMultipleEvents_ShouldHandleEachEvent()
    {
        // Arrange
        var events = new[]
        {
            new UserLoggedOutEvent { UserId = Guid.NewGuid(), LoggedOutAt = DateTime.UtcNow },
            new UserLoggedOutEvent { UserId = Guid.NewGuid(), LoggedOutAt = DateTime.UtcNow },
            new UserLoggedOutEvent { UserId = Guid.NewGuid(), LoggedOutAt = DateTime.UtcNow }
        };

        // Act
        foreach (var userEvent in events)
        {
            await _handler.HandleAsync(userEvent);
        }

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Exactly(3));
    }
}
