using System.Text.RegularExpressions;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Domain.ValueObjects;

public sealed record PhoneNumber : IEquatable<PhoneNumber>
{
    private static readonly Regex PhoneRegex = new(
        @"^\+?[1-9]\d{1,14}$",
        RegexOptions.Compiled);

    public string CountryCode { get; init; }
    public string Number { get; init; }

    private PhoneNumber(string countryCode, string number)
    {
        CountryCode = countryCode;
        Number = number;
    }

    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<PhoneNumber>.Failure("Phone number is required", "INVALID_PHONE");

        // Remove all non-digit characters except +
        var cleaned = Regex.Replace(value, @"[^\d+]", "");

        if (!PhoneRegex.IsMatch(cleaned))
            return Result<PhoneNumber>.Failure("Invalid phone number format", "INVALID_PHONE_FORMAT");

        string countryCode = "+1"; // Default
        string number = cleaned;

        if (cleaned.StartsWith("+"))
        {
            // Extract country code (1-3 digits after +)
            var match = Regex.Match(cleaned, @"^\+(\d{1,3})(\d+)$");
            if (match.Success)
            {
                countryCode = "+" + match.Groups[1].Value;
                number = match.Groups[2].Value;
            }
        }

        return Result<PhoneNumber>.Success(new PhoneNumber(countryCode, number));
    }

    public string GetFullNumber() => $"{CountryCode}{Number}";

    public bool Equals(PhoneNumber? other)
    {
        if (other is null) return false;
        return CountryCode == other.CountryCode && Number == other.Number;
    }

    public override int GetHashCode() => HashCode.Combine(CountryCode, Number);

    public override string ToString() => GetFullNumber();
}
