using AutoMapper;
using Moq;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Application.Features.Customers.Queries.GetCustomers;
using ShahdCooperative.Domain.Entities;
using ShahdCooperative.Domain.Interfaces.Repositories;

namespace ShahdCooperative.Application.Tests.Features.Customers.Queries;

public class GetCustomersQueryHandlerTests
{
    private readonly Mock<ICustomerRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetCustomersQueryHandler _handler;

    public GetCustomersQueryHandlerTests()
    {
        _mockRepository = new Mock<ICustomerRepository>();
        _mockMapper = new Mock<IMapper>();
        _handler = new GetCustomersQueryHandler(_mockRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_ReturnsAllCustomers()
    {
        // Arrange
        var query = new GetCustomersQuery();
        var customers = new List<Customer>
        {
            Customer.Create("auth1", "John Doe", "john@example.com"),
            Customer.Create("auth2", "Jane Smith", "jane@example.com")
        };

        var customerDtos = new List<CustomerDto>
        {
            new CustomerDto { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com" },
            new CustomerDto { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "jane@example.com" }
        };

        _mockRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);
        _mockMapper.Setup(x => x.Map<IEnumerable<CustomerDto>>(It.IsAny<IEnumerable<Customer>>()))
            .Returns(customerDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
        _mockRepository.Verify(x => x.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoCustomers_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetCustomersQuery();
        var customers = new List<Customer>();
        var customerDtos = new List<CustomerDto>();

        _mockRepository.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(customers);
        _mockMapper.Setup(x => x.Map<IEnumerable<CustomerDto>>(It.IsAny<IEnumerable<Customer>>()))
            .Returns(customerDtos);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }
}
