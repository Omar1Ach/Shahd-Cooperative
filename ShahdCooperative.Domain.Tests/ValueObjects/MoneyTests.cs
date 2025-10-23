using FluentAssertions;
using ShahdCooperative.Domain.ValueObjects;

namespace ShahdCooperative.Domain.Tests.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidAmountAndCurrency_ShouldSucceed()
    {
        // Arrange
        var amount = 100.50m;
        var currency = "USD";

        // Act
        var result = Money.Create(amount, currency);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Amount.Should().Be(amount);
        result.Value.Currency.Should().Be("USD");
    }

    [Fact]
    public void Create_WithLowercaseCurrency_ShouldConvertToUppercase()
    {
        // Arrange
        var amount = 50.25m;
        var currency = "eur";

        // Act
        var result = Money.Create(amount, currency);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Currency.Should().Be("EUR");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldFail()
    {
        // Arrange
        var amount = -10.00m;
        var currency = "USD";

        // Act
        var result = Money.Create(amount, currency);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Amount cannot be negative");
        result.ErrorCode.Should().Be("INVALID_AMOUNT");
    }

    [Fact]
    public void Create_WithNullCurrency_ShouldFail()
    {
        // Arrange
        var amount = 100.00m;
        string currency = null!;

        // Act
        var result = Money.Create(amount, currency);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Currency is required");
        result.ErrorCode.Should().Be("INVALID_CURRENCY");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void Create_WithWhitespaceCurrency_ShouldFail(string currency)
    {
        // Arrange
        var amount = 100.00m;

        // Act
        var result = Money.Create(amount, currency);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Currency is required");
    }

    [Theory]
    [InlineData("US")]
    [InlineData("USDT")]
    [InlineData("E")]
    public void Create_WithInvalidCurrencyLength_ShouldFail(string currency)
    {
        // Arrange
        var amount = 100.00m;

        // Act
        var result = Money.Create(amount, currency);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Currency must be a 3-letter ISO code");
        result.ErrorCode.Should().Be("INVALID_CURRENCY_FORMAT");
    }

    [Fact]
    public void Addition_WithSameCurrency_ShouldSucceed()
    {
        // Arrange
        var money1 = Money.Create(100.00m, "USD").Value!;
        var money2 = Money.Create(50.00m, "USD").Value!;

        // Act
        var result = money1 + money2;

        // Assert
        result.Amount.Should().Be(150.00m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Addition_WithDifferentCurrencies_ShouldThrowException()
    {
        // Arrange
        var money1 = Money.Create(100.00m, "USD").Value!;
        var money2 = Money.Create(50.00m, "EUR").Value!;

        // Act
        var act = () => money1 + money2;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot add money with different currencies*");
    }

    [Fact]
    public void Subtraction_WithSameCurrency_ShouldSucceed()
    {
        // Arrange
        var money1 = Money.Create(100.00m, "USD").Value!;
        var money2 = Money.Create(30.00m, "USD").Value!;

        // Act
        var result = money1 - money2;

        // Assert
        result.Amount.Should().Be(70.00m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Subtraction_WithDifferentCurrencies_ShouldThrowException()
    {
        // Arrange
        var money1 = Money.Create(100.00m, "USD").Value!;
        var money2 = Money.Create(30.00m, "EUR").Value!;

        // Act
        var act = () => money1 - money2;

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot subtract money with different currencies*");
    }

    [Fact]
    public void Multiplication_WithPositiveMultiplier_ShouldSucceed()
    {
        // Arrange
        var money = Money.Create(50.00m, "USD").Value!;
        var multiplier = 3m;

        // Act
        var result = money * multiplier;

        // Assert
        result.Amount.Should().Be(150.00m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Division_WithNonZeroDivisor_ShouldSucceed()
    {
        // Arrange
        var money = Money.Create(100.00m, "USD").Value!;
        var divisor = 4m;

        // Act
        var result = money / divisor;

        // Assert
        result.Amount.Should().Be(25.00m);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Division_WithZeroDivisor_ShouldThrowException()
    {
        // Arrange
        var money = Money.Create(100.00m, "USD").Value!;
        var divisor = 0m;

        // Act
        var act = () => money / divisor;

        // Assert
        act.Should().Throw<DivideByZeroException>();
    }

    [Fact]
    public void Equals_WithSameAmountAndCurrency_ShouldReturnTrue()
    {
        // Arrange
        var money1 = Money.Create(100.00m, "USD").Value!;
        var money2 = Money.Create(100.00m, "USD").Value!;

        // Act & Assert
        money1.Equals(money2).Should().BeTrue();
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentAmount_ShouldReturnFalse()
    {
        // Arrange
        var money1 = Money.Create(100.00m, "USD").Value!;
        var money2 = Money.Create(50.00m, "USD").Value!;

        // Act & Assert
        money1.Equals(money2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentCurrency_ShouldReturnFalse()
    {
        // Arrange
        var money1 = Money.Create(100.00m, "USD").Value!;
        var money2 = Money.Create(100.00m, "EUR").Value!;

        // Act & Assert
        money1.Equals(money2).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldFormatCorrectly()
    {
        // Arrange
        var money = Money.Create(1234.56m, "USD").Value!;

        // Act
        var result = money.ToString();

        // Assert
        result.Should().Contain("1").And.Contain("234").And.Contain("56").And.Contain("USD");
    }
}
