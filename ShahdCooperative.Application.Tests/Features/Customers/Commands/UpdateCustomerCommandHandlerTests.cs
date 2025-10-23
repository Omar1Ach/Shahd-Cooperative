using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Application.Features.Customers.Commands.UpdateCustomer;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Customers.Commands;

public class UpdateCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UpdateCustomerCommandHandler _handler;

    public UpdateCustomerCommandHandlerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new UpdateCustomerCommandHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var dto = new UpdateCustomerDto
        {
            Id = customerId,
            Name = "John Updated",
            Email = "john.updated@example.com",
            Phone = "+1234567890"
        };

        var command = new UpdateCustomerCommand(dto);
        var customer = Customer.Create("auth123", "John", "john@example.com");
        var customerDto = new CustomerDto { Id = customerId, Name = dto.Name, Email = dto.Email };

        _mockRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockRepository.Setup(x => x.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mockMapper.Setup(x => x.Map<CustomerDto>(It.IsAny<Customer>()))
            .Returns(customerDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(dto.Name, result.Value.Name);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ReturnsFailureResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var dto = new UpdateCustomerDto
        {
            Id = customerId,
            Name = "John Updated",
            Email = "john.updated@example.com"
        };

        var command = new UpdateCustomerCommand(dto);

        _mockRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
        _mockRepository.Verify(x => x.UpdateAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
