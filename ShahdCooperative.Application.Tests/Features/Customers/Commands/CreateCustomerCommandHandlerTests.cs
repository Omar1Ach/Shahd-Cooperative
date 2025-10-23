using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Application.Features.Customers.Commands.CreateCustomer;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Customers.Commands;

public class CreateCustomerCommandHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CreateCustomerCommandHandler _handler;

    public CreateCustomerCommandHandlerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new CreateCustomerCommandHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccessResult()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            ExternalAuthId = "auth123",
            Name = "John Doe",
            Email = "john@example.com",
            Phone = "+1234567890"
        };

        var command = new CreateCustomerCommand(dto);
        var customer = Customer.Create(dto.ExternalAuthId, dto.Name, dto.Email, dto.Phone);
        var customerDto = new CustomerDto { Id = Guid.NewGuid(), Name = dto.Name, Email = dto.Email };

        _mockRepository.Setup(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockMapper.Setup(x => x.Map<CustomerDto>(It.IsAny<Customer>()))
            .Returns(customerDto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(dto.Name, result.Value.Name);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidData_ReturnsFailureResult()
    {
        // Arrange
        var dto = new CreateCustomerDto
        {
            ExternalAuthId = "",  // Invalid - empty
            Name = "John Doe",
            Email = "john@example.com"
        };

        var command = new CreateCustomerCommand(dto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("VALIDATION_ERROR", result.ErrorCode);
        _mockRepository.Verify(x => x.AddAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
