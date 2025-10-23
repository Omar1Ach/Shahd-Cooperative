using FluentAssertions;
using ShahdCooperative.Application.DTOs.Products;
using ShahdCooperative.Application.Features.Products.Commands.CreateProduct;

namespace ShahdCooperative.Application.Tests.Validators;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldPass()
    {
        // Arrange
        var command = new CreateProductCommand(
            new CreateProductDto
            {
                Name = "Test Product",
                SKU = "SKU12345",
                Category = "Test Category",
                Type = "Honey",
                Price = 100.00m,
                Currency = "USD",
                StockQuantity = 10,
                ThresholdLevel = 5
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyName_ShouldFail()
    {
        // Arrange
        var command = new CreateProductCommand(
            new CreateProductDto
            {
                Name = "",
                SKU = "SKU12345",
                Category = "Test Category",
                Type = "Honey",
                Price = 100.00m,
                Currency = "USD",
                StockQuantity = 10
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Name"));
    }

    [Fact]
    public void Validate_WithTooLongName_ShouldFail()
    {
        // Arrange
        var command = new CreateProductCommand(
            new CreateProductDto
            {
                Name = new string('a', 201),
                SKU = "SKU12345",
                Category = "Test Category",
                Type = "Honey",
                Price = 100.00m,
                Currency = "USD",
                StockQuantity = 10
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("200 characters"));
    }

    [Fact]
    public void Validate_WithZeroPrice_ShouldFail()
    {
        // Arrange
        var command = new CreateProductCommand(
            new CreateProductDto
            {
                Name = "Test Product",
                SKU = "SKU12345",
                Category = "Test Category",
                Type = "Honey",
                Price = 0,
                Currency = "USD",
                StockQuantity = 10
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Price"));
    }

    [Fact]
    public void Validate_WithInvalidCurrency_ShouldFail()
    {
        // Arrange
        var command = new CreateProductCommand(
            new CreateProductDto
            {
                Name = "Test Product",
                SKU = "SKU12345",
                Category = "Test Category",
                Type = "Honey",
                Price = 100.00m,
                Currency = "US",
                StockQuantity = 10
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage.Contains("3-letter ISO code"));
    }

    [Fact]
    public void Validate_WithNegativeStock_ShouldFail()
    {
        // Arrange
        var command = new CreateProductCommand(
            new CreateProductDto
            {
                Name = "Test Product",
                SKU = "SKU12345",
                Category = "Test Category",
                Type = "Honey",
                Price = 100.00m,
                Currency = "USD",
                StockQuantity = -5
            });

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("StockQuantity"));
    }
}
