using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ShahdCooperative.API.Controllers;
using ShahdCooperative.Application.DTOs.Customers;
using ShahdCooperative.Application.Features.Customers.Commands.CreateCustomer;
using ShahdCooperative.Application.Features.Customers.Commands.DeleteCustomer;
using ShahdCooperative.Application.Features.Customers.Commands.UpdateCustomer;
using ShahdCooperative.Application.Features.Customers.Queries.GetCustomerById;
using ShahdCooperative.Application.Features.Customers.Queries.GetCustomers;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.API.Tests.Controllers;

public class CustomersControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILogger<CustomersController>> _mockLogger;
    private readonly CustomersController _controller;

    public CustomersControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockLogger = new Mock<ILogger<CustomersController>>();
        _controller = new CustomersController(_mockMediator.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetCustomers_ReturnsOkResult_WithCustomers()
    {
        // Arrange
        var customers = new List<CustomerDto>
        {
            new CustomerDto { Id = Guid.NewGuid(), Name = "John Doe", Email = "john@example.com" },
            new CustomerDto { Id = Guid.NewGuid(), Name = "Jane Smith", Email = "jane@example.com" }
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<GetCustomersQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IEnumerable<CustomerDto>>.Success(customers));

        // Act
        var result = await _controller.GetCustomers(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCustomers = Assert.IsAssignableFrom<IEnumerable<CustomerDto>>(okResult.Value);
        Assert.Equal(2, returnedCustomers.Count());
    }

    [Fact]
    public async Task GetCustomer_ExistingId_ReturnsOkResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var customerDto = new CustomerDto { Id = customerId, Name = "John Doe", Email = "john@example.com" };

        _mockMediator.Setup(x => x.Send(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CustomerDto>.Success(customerDto));

        // Act
        var result = await _controller.GetCustomer(customerId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCustomer = Assert.IsType<CustomerDto>(okResult.Value);
        Assert.Equal(customerId, returnedCustomer.Id);
    }

    [Fact]
    public async Task GetCustomer_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _mockMediator.Setup(x => x.Send(It.IsAny<GetCustomerByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CustomerDto>.Failure("Customer not found", "NOT_FOUND"));

        // Act
        var result = await _controller.GetCustomer(customerId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task CreateCustomer_ValidDto_ReturnsCreatedAtAction()
    {
        // Arrange
        var createDto = new CreateCustomerDto
        {
            ExternalAuthId = "auth123",
            Name = "John Doe",
            Email = "john@example.com"
        };

        var customerDto = new CustomerDto
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Email = createDto.Email
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<CreateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CustomerDto>.Success(customerDto));

        // Act
        var result = await _controller.CreateCustomer(createDto, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedCustomer = Assert.IsType<CustomerDto>(createdResult.Value);
        Assert.Equal(customerDto.Id, returnedCustomer.Id);
    }

    [Fact]
    public async Task UpdateCustomer_ValidDto_ReturnsOkResult()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var updateDto = new UpdateCustomerDto
        {
            Id = customerId,
            Name = "John Updated",
            Email = "john.updated@example.com"
        };

        var customerDto = new CustomerDto
        {
            Id = customerId,
            Name = updateDto.Name,
            Email = updateDto.Email
        };

        _mockMediator.Setup(x => x.Send(It.IsAny<UpdateCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CustomerDto>.Success(customerDto));

        // Act
        var result = await _controller.UpdateCustomer(customerId, updateDto, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCustomer = Assert.IsType<CustomerDto>(okResult.Value);
        Assert.Equal(updateDto.Name, returnedCustomer.Name);
    }

    [Fact]
    public async Task UpdateCustomer_IdMismatch_ReturnsBadRequest()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var differentId = Guid.NewGuid();
        var updateDto = new UpdateCustomerDto
        {
            Id = differentId,
            Name = "John Updated",
            Email = "john.updated@example.com"
        };

        // Act
        var result = await _controller.UpdateCustomer(customerId, updateDto, CancellationToken.None);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task DeleteCustomer_ExistingId_ReturnsNoContent()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _mockMediator.Setup(x => x.Send(It.IsAny<DeleteCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        // Act
        var result = await _controller.DeleteCustomer(customerId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteCustomer_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();

        _mockMediator.Setup(x => x.Send(It.IsAny<DeleteCustomerCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Customer not found", "NOT_FOUND"));

        // Act
        var result = await _controller.DeleteCustomer(customerId, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
