using FluentAssertions;
using ShahdCooperative.Application.DTOs.Orders;
using ShahdCooperative.Application.Features.Orders.Commands.CreateOrder;

namespace ShahdCooperative.Application.Tests.Validators;

public class CreateOrderCommandValidatorTests
{
    private readonly CreateOrderCommandValidator _validator;

    public CreateOrderCommandValidatorTests()
    {
        _validator = new CreateOrderCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateOrderCommand(
            new CreateOrderDto
            {
                CustomerId = Guid.NewGuid(),
                OrderItems = new List<CreateOrderItemDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 2 }
                },
                ShippingCity = "Springfield",
                ShippingCountry = "USA"
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCustomerId_ShouldFail()
    {
        // Arrange
        var command = new CreateOrderCommand(
            new CreateOrderDto
            {
                CustomerId = Guid.Empty,
                OrderItems = new List<CreateOrderItemDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 2 }
                }
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Customer ID is required"));
    }

    [Fact]
    public void Validate_WithEmptyOrderItems_ShouldFail()
    {
        // Arrange
        var command = new CreateOrderCommand(
            new CreateOrderDto
            {
                CustomerId = Guid.NewGuid(),
                OrderItems = new List<CreateOrderItemDto>()
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("at least one item"));
    }

    [Fact]
    public void Validate_WithInvalidProductId_ShouldFail()
    {
        // Arrange
        var command = new CreateOrderCommand(
            new CreateOrderDto
            {
                CustomerId = Guid.NewGuid(),
                OrderItems = new List<CreateOrderItemDto>
                {
                    new() { ProductId = Guid.Empty, Quantity = 2 }
                }
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Product ID is required"));
    }

    [Fact]
    public void Validate_WithZeroQuantity_ShouldFail()
    {
        // Arrange
        var command = new CreateOrderCommand(
            new CreateOrderDto
            {
                CustomerId = Guid.NewGuid(),
                OrderItems = new List<CreateOrderItemDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 0 }
                }
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("Quantity must be greater than 0"));
    }

    [Fact]
    public void Validate_WithTooLongCity_ShouldFail()
    {
        // Arrange
        var command = new CreateOrderCommand(
            new CreateOrderDto
            {
                CustomerId = Guid.NewGuid(),
                OrderItems = new List<CreateOrderItemDto>
                {
                    new() { ProductId = Guid.NewGuid(), Quantity = 2 }
                },
                ShippingCity = new string('a', 101)
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("100 characters"));
    }
}
