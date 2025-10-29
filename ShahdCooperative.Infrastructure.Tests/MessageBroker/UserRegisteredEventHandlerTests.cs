using Microsoft.Extensions.Logging;
using Moq;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces;
using ShahdCooperative.Domain.Interfaces.Repositories;
using ShahdCooperative.Infrastructure.MessageBroker.Events;
using ShahdCooperative.Infrastructure.MessageBroker.Handlers;

namespace ShahdCooperative.Infrastructure.Tests.MessageBroker;

public class UserRegisteredEventHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockCustomerRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<UserRegisteredEventHandler>> _mockLogger;
    private readonly UserRegisteredEventHandler _handler;

    public UserRegisteredEventHandlerTests()
    {
        _mockCustomerRepository = new Mock<ICustomerRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<UserRegisteredEventHandler>>();
        _handler = new UserRegisteredEventHandler(
            _mockCustomerRepository.Object,
            _mockUnitOfWork.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task HandleAsync_WhenNewUser_ShouldCreateCustomer()
    {
        // Arrange
        var userEvent = new UserRegisteredEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Role = "Customer",
            RegisteredAt = DateTime.UtcNow
        };

        _mockCustomerRepository
            .Setup(x => x.GetByAuthIdAsync(userEvent.UserId.ToString(), default))
            .ReturnsAsync((Customer?)null);

        _mockCustomerRepository
            .Setup(x => x.AddAsync(It.IsAny<Customer>(), default))
            .ReturnsAsync((Customer c, CancellationToken ct) => c);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _handler.HandleAsync(userEvent);

        // Assert
        _mockCustomerRepository.Verify(
            x => x.GetByAuthIdAsync(userEvent.UserId.ToString(), default),
            Times.Once);

        _mockCustomerRepository.Verify(
            x => x.AddAsync(It.Is<Customer>(c =>
                c.ExternalAuthId == userEvent.UserId.ToString() &&
                c.Email == userEvent.Email),
                default),
            Times.Once);

        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(default),
            Times.Once);
    }

    [Fact]
    public async Task HandleAsync_WhenCustomerAlreadyExists_ShouldNotCreateDuplicate()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var existingCustomer = Customer.Create(
            userId.ToString(),
            "Existing User",
            "existing@example.com");

        var userEvent = new UserRegisteredEvent
        {
            UserId = userId,
            Email = "existing@example.com",
            Role = "Customer",
            RegisteredAt = DateTime.UtcNow
        };

        _mockCustomerRepository
            .Setup(x => x.GetByAuthIdAsync(userId.ToString(), default))
            .ReturnsAsync(existingCustomer);

        // Act
        await _handler.HandleAsync(userEvent);

        // Assert
        _mockCustomerRepository.Verify(
            x => x.GetByAuthIdAsync(userId.ToString(), default),
            Times.Once);

        _mockCustomerRepository.Verify(
            x => x.AddAsync(It.IsAny<Customer>(), default),
            Times.Never);

        _mockUnitOfWork.Verify(
            x => x.SaveChangesAsync(default),
            Times.Never);
    }

    [Fact]
    public async Task HandleAsync_WhenEmailHasLocalPart_ShouldExtractNameFromEmail()
    {
        // Arrange
        var userEvent = new UserRegisteredEvent
        {
            UserId = Guid.NewGuid(),
            Email = "john.doe@example.com",
            Role = "Customer",
            RegisteredAt = DateTime.UtcNow
        };

        _mockCustomerRepository
            .Setup(x => x.GetByAuthIdAsync(userEvent.UserId.ToString(), default))
            .ReturnsAsync((Customer?)null);

        Customer? capturedCustomer = null;
        _mockCustomerRepository
            .Setup(x => x.AddAsync(It.IsAny<Customer>(), default))
            .Callback<Customer, CancellationToken>((c, _) => capturedCustomer = c)
            .ReturnsAsync((Customer c, CancellationToken ct) => c);

        _mockUnitOfWork
            .Setup(x => x.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        await _handler.HandleAsync(userEvent);

        // Assert
        Assert.NotNull(capturedCustomer);
        Assert.Equal("john.doe", capturedCustomer.Name);
        Assert.Equal(userEvent.Email, capturedCustomer.Email);
    }

    [Fact]
    public async Task HandleAsync_WhenRepositoryThrowsException_ShouldLogErrorAndRethrow()
    {
        // Arrange
        var userEvent = new UserRegisteredEvent
        {
            UserId = Guid.NewGuid(),
            Email = "test@example.com",
            Role = "Customer",
            RegisteredAt = DateTime.UtcNow
        };

        var expectedException = new Exception("Database error");

        _mockCustomerRepository
            .Setup(x => x.GetByAuthIdAsync(userEvent.UserId.ToString(), default))
            .ThrowsAsync(expectedException);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _handler.HandleAsync(userEvent));

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                expectedException,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
