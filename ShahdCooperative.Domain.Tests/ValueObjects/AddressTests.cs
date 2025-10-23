using FluentAssertions;
using ShahdCooperative.Domain.ValueObjects;

namespace ShahdCooperative.Domain.Tests.ValueObjects;

public class AddressTests
{
    [Fact]
    public void Create_WithAllValidFields_ShouldSucceed()
    {
        // Arrange
        var street = "123 Main St";
        var city = "Springfield";
        var state = "IL";
        var postalCode = "62701";
        var country = "USA";

        // Act
        var result = Address.Create(street, city, state, postalCode, country);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Street.Should().Be(street);
        result.Value.City.Should().Be(city);
        result.Value.State.Should().Be(state);
        result.Value.PostalCode.Should().Be(postalCode);
        result.Value.Country.Should().Be(country);
    }

    [Fact]
    public void Create_WithNullStreet_ShouldFail()
    {
        // Act
        var result = Address.Create(null!, "City", "State", "12345", "Country");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Street is required");
    }

    [Fact]
    public void Create_WithNullCity_ShouldFail()
    {
        // Act
        var result = Address.Create("Street", null!, "State", "12345", "Country");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("City is required");
    }

    [Fact]
    public void Create_WithNullState_ShouldFail()
    {
        // Act
        var result = Address.Create("Street", "City", null!, "12345", "Country");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("State is required");
    }

    [Fact]
    public void Create_WithNullPostalCode_ShouldFail()
    {
        // Act
        var result = Address.Create("Street", "City", "State", null!, "Country");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Postal code is required");
    }

    [Fact]
    public void Create_WithNullCountry_ShouldFail()
    {
        // Act
        var result = Address.Create("Street", "City", "State", "12345", null!);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Country is required");
    }

    [Fact]
    public void Equals_WithSameAddress_ShouldReturnTrue()
    {
        // Arrange
        var address1 = Address.Create("123 Main St", "Springfield", "IL", "62701", "USA").Value!;
        var address2 = Address.Create("123 Main St", "Springfield", "IL", "62701", "USA").Value!;

        // Act & Assert
        address1.Equals(address2).Should().BeTrue();
        (address1 == address2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentStreet_ShouldReturnFalse()
    {
        // Arrange
        var address1 = Address.Create("123 Main St", "Springfield", "IL", "62701", "USA").Value!;
        var address2 = Address.Create("456 Oak Ave", "Springfield", "IL", "62701", "USA").Value!;

        // Act & Assert
        address1.Equals(address2).Should().BeFalse();
    }

    [Fact]
    public void ToString_ShouldFormatAddressCorrectly()
    {
        // Arrange
        var address = Address.Create("123 Main St", "Springfield", "IL", "62701", "USA").Value!;

        // Act
        var result = address.ToString();

        // Assert
        result.Should().Contain("123 Main St");
        result.Should().Contain("Springfield");
        result.Should().Contain("IL");
        result.Should().Contain("62701");
        result.Should().Contain("USA");
    }
}
