using System.Text.RegularExpressions;
using ShahdCooperative.Domain.Common;

namespace ShahdCooperative.Domain.ValueObjects;

public sealed record OrderNumber : IEquatable<OrderNumber>
{
    private static readonly Regex OrderNumberRegex = new(
        @"^ORD-\d{8}-\d{5}$",
        RegexOptions.Compiled);

    public string Value { get; init; }

    private OrderNumber(string value)
    {
        Value = value;
    }

    public static Result<OrderNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<OrderNumber>.Failure("Order number is required", "INVALID_ORDER_NUMBER");

        if (!OrderNumberRegex.IsMatch(value))
            return Result<OrderNumber>.Failure(
                "Order number must be in format: ORD-YYYYMMDD-NNNNN",
                "INVALID_ORDER_NUMBER_FORMAT");

        return Result<OrderNumber>.Success(new OrderNumber(value.ToUpperInvariant()));
    }

    public static OrderNumber Generate()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Random.Shared.Next(0, 99999).ToString("D5");
        var value = $"ORD-{datePart}-{randomPart}";
        return new OrderNumber(value);
    }

    public bool Equals(OrderNumber? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static implicit operator string(OrderNumber orderNumber) => orderNumber.Value;
}
