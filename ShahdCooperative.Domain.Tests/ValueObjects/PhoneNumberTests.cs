using FluentAssertions;
using ShahdCooperative.Domain.ValueObjects;

namespace ShahdCooperative.Domain.Tests.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+1234567890")]
    [InlineData("+12345678901234")]
    [InlineData("+999999999")]
    public void Create_WithValidPhoneNumber_ShouldSucceed(string phoneNumber)
    {
        // Act
        var result = PhoneNumber.Create(phoneNumber);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.GetFullNumber().Should().Be(phoneNumber);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void Create_WithNullOrWhitespace_ShouldFail(string phoneNumber)
    {
        // Act
        var result = PhoneNumber.Create(phoneNumber);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Phone number is required");
    }

    [Theory]
    [InlineData("+")]
    [InlineData("+abc")]
    [InlineData("+0123456789")]
    public void Create_WithInvalidFormat_ShouldFail(string phoneNumber)
    {
        // Act
        var result = PhoneNumber.Create(phoneNumber);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid phone number format");
    }

    [Fact]
    public void Equals_WithSamePhoneNumber_ShouldReturnTrue()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("+1234567890").Value!;
        var phone2 = PhoneNumber.Create("+1234567890").Value!;

        // Act & Assert
        phone1.Equals(phone2).Should().BeTrue();
        (phone1 == phone2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentPhoneNumber_ShouldReturnFalse()
    {
        // Arrange
        var phone1 = PhoneNumber.Create("+1234567890").Value!;
        var phone2 = PhoneNumber.Create("+9876543210").Value!;

        // Act & Assert
        phone1.Equals(phone2).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnPhoneNumberValue()
    {
        // Arrange
        var phone = PhoneNumber.Create("+1234567890").Value!;

        // Act
        var result = phone.ToString();

        // Assert
        result.Should().Be("+1234567890");
    }
}
