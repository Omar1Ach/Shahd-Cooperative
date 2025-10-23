using FluentAssertions;
using ShahdCooperative.Domain.ValueObjects;

namespace ShahdCooperative.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name@domain.co.uk")]
    [InlineData("user+tag@example.com")]
    [InlineData("firstname.lastname@company.org")]
    public void Create_WithValidEmail_ShouldSucceed(string emailAddress)
    {
        // Act
        var result = Email.Create(emailAddress);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Value.Should().Be(emailAddress.ToLowerInvariant());
    }

    [Fact]
    public void Create_WithUppercaseEmail_ShouldConvertToLowercase()
    {
        // Arrange
        var emailAddress = "TEST@EXAMPLE.COM";

        // Act
        var result = Email.Create(emailAddress);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Value.Should().Be("test@example.com");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void Create_WithNullOrWhitespace_ShouldFail(string emailAddress)
    {
        // Act
        var result = Email.Create(emailAddress);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Email is required");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("invalid@")]
    [InlineData("@example.com")]
    [InlineData("invalid@@example.com")]
    [InlineData("invalid.example.com")]
    [InlineData("invalid @example.com")]
    public void Create_WithInvalidFormat_ShouldFail(string emailAddress)
    {
        // Act
        var result = Email.Create(emailAddress);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Invalid email format");
    }

    [Fact]
    public void Equals_WithSameEmail_ShouldReturnTrue()
    {
        // Arrange
        var email1 = Email.Create("test@example.com").Value!;
        var email2 = Email.Create("test@example.com").Value!;

        // Act & Assert
        email1.Equals(email2).Should().BeTrue();
        (email1 == email2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentEmail_ShouldReturnFalse()
    {
        // Arrange
        var email1 = Email.Create("test1@example.com").Value!;
        var email2 = Email.Create("test2@example.com").Value!;

        // Act & Assert
        email1.Equals(email2).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldReturnEmailValue()
    {
        // Arrange
        var email = Email.Create("test@example.com").Value!;

        // Act
        var result = email.ToString();

        // Assert
        result.Should().Be("test@example.com");
    }
}
