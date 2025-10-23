using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Application.Features.Customers.Queries.GetCustomerById;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Customers.Queries;

public class GetCustomerByIdQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetCustomerByIdQueryHandler _handler;

    public GetCustomerByIdQueryHandlerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetCustomerByIdQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_CustomerExists_ReturnsSuccessResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var query = new GetCustomerByIdQuery(customerId);
        var customer = Customer.Create("auth123", "John Doe", "john@example.com");
        var customerDto = new CustomerDto { Id = customerId, Name = "John Doe", Email = "john@example.com" };

        _mockRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(customer);
        _mockMapper.Setup(x => x.Map<CustomerDto>(It.IsAny<Customer>()))
            .Returns(customerDto);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(customerId, result.Value.Id);
        _mockRepository.Verify(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CustomerNotFound_ReturnsFailureResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var query = new GetCustomerByIdQuery(customerId);

        _mockRepository.Setup(x => x.GetByIdAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Customer?)null);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("NOT_FOUND", result.ErrorCode);
        Assert.Null(result.Value);
    }
}
