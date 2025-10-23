using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Domain.ValueObjects;

public sealed record Address : IEquatable<Address>
{
    public string Street { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string PostalCode { get; init; }
    public string Country { get; init; }

    private Address(string street, string city, string state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    public static Result<Address> Create(
        string street,
        string city,
        string state,
        string postalCode,
        string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            return Result<Address>.Failure("Street is required", "INVALID_ADDRESS");

        if (string.IsNullOrWhiteSpace(city))
            return Result<Address>.Failure("City is required", "INVALID_ADDRESS");

        if (string.IsNullOrWhiteSpace(state))
            return Result<Address>.Failure("State is required", "INVALID_ADDRESS");

        if (string.IsNullOrWhiteSpace(postalCode))
            return Result<Address>.Failure("Postal code is required", "INVALID_ADDRESS");

        if (string.IsNullOrWhiteSpace(country))
            return Result<Address>.Failure("Country is required", "INVALID_ADDRESS");

        if (street.Length > 200)
            return Result<Address>.Failure("Street cannot exceed 200 characters", "ADDRESS_TOO_LONG");

        if (city.Length > 100)
            return Result<Address>.Failure("City cannot exceed 100 characters", "ADDRESS_TOO_LONG");

        if (state.Length > 100)
            return Result<Address>.Failure("State cannot exceed 100 characters", "ADDRESS_TOO_LONG");

        if (postalCode.Length > 20)
            return Result<Address>.Failure("Postal code cannot exceed 20 characters", "ADDRESS_TOO_LONG");

        if (country.Length > 100)
            return Result<Address>.Failure("Country cannot exceed 100 characters", "ADDRESS_TOO_LONG");

        return Result<Address>.Success(new Address(
            street.Trim(),
            city.Trim(),
            state.Trim(),
            postalCode.Trim(),
            country.Trim()));
    }

    public bool Equals(Address? other)
    {
        if (other is null) return false;
        return Street == other.Street &&
               City == other.City &&
               State == other.State &&
               PostalCode == other.PostalCode &&
               Country == other.Country;
    }

    public override int GetHashCode() =>
        HashCode.Combine(Street, City, State, PostalCode, Country);

    public override string ToString() =>
        $"{Street}, {City}, {State} {PostalCode}, {Country}";
}
