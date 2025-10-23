using System.Text.RegularExpressions;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Domain.ValueObjects;

public sealed record Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; init; }

    private Email(string value)
    {
        Value = value;
    }

    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Email>.Failure("Email is required", "INVALID_EMAIL");

        if (value.Length > 255)
            return Result<Email>.Failure("Email cannot exceed 255 characters", "EMAIL_TOO_LONG");

        if (!EmailRegex.IsMatch(value))
            return Result<Email>.Failure("Invalid email format", "INVALID_EMAIL_FORMAT");

        return Result<Email>.Success(new Email(value.ToLowerInvariant()));
    }

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
