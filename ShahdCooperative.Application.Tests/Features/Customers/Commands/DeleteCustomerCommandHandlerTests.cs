using Moq;
using ShahdCooperative.Application.Features.Customers.Commands.DeleteCustomer;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Customers.Commands;

public class DeleteCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly DeleteCustomerCommandHandler _handler;

    public DeleteCustomerCommandHandlerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _handler = new DeleteCustomerCommandHandler(_mockRepository.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new DeleteCustomerCommand(customerId);
        var customer = Customer.Create("auth123", "John Doe", "john@example.com");

        _mockRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockRepository.Setup(x => x.DeleteAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ReturnsFailureResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var command = new DeleteCustomerCommand(customerId);

        _mockRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
        _mockRepository.Verify(x => x.DeleteAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
