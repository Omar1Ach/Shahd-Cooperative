namespace ShahdCooperative.Domain.Entities;

public class Customer : BaseEntity
{
    public string ExternalAuthId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? Phone { get; private set; }
    public string? Street { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? PostalCode { get; private set; }
    public string? Country { get; private set; }
    public int LoyaltyPoints { get; private set; } = 0;
    public bool IsActive { get; private set; } = true;
    public DateTime DateJoined { get; private set; } = DateTime.UtcNow;

    // Navigation properties
    public ICollection<Order> Orders { get; private set; } = new List<Order>();
    public ICollection<Feedback> Feedbacks { get; private set; } = new List<Feedback>();

    // Parameterless constructor for EF Core
    private Customer() { }

    // Factory method
    public static Customer Create(
        string externalAuthId,
        string name,
        string email,
        string? phone = null,
        string? street = null,
        string? city = null,
        string? state = null,
        string? postalCode = null,
        string? country = null)
    {
        if (string.IsNullOrWhiteSpace(externalAuthId))
            throw new ArgumentException("External auth ID cannot be empty", nameof(externalAuthId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        return new Customer
        {
            ExternalAuthId = externalAuthId,
            Name = name,
            Email = email,
            Phone = phone,
            Street = street,
            City = city,
            State = state,
            PostalCode = postalCode,
            Country = country,
            DateJoined = DateTime.UtcNow,
            IsActive = true,
            LoyaltyPoints = 0
        };
    }

    // Business methods
    public void AddLoyaltyPoints(int points)
    {
        if (points < 0)
            throw new ArgumentException("Points cannot be negative", nameof(points));

        if (!IsActive)
            throw new InvalidOperationException("Cannot add loyalty points to inactive customer");

        LoyaltyPoints += points;
    }

    public void RedeemLoyaltyPoints(int points)
    {
        if (points < 0)
            throw new ArgumentException("Points cannot be negative", nameof(points));

        if (!IsActive)
            throw new InvalidOperationException("Cannot redeem loyalty points for inactive customer");

        if (points > LoyaltyPoints)
            throw new InvalidOperationException($"Insufficient loyalty points. Available: {LoyaltyPoints}, Requested: {points}");

        LoyaltyPoints -= points;
    }

    public bool CanPlaceOrder() => IsActive;

    public void DeactivateAccount()
    {
        IsActive = false;
    }

    public void ActivateAccount()
    {
        IsActive = true;
    }

    public void UpdateProfile(
        string name,
        string email,
        string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        Name = name;
        Email = email;
        Phone = phone;
    }

    public void UpdateAddress(
        string? street,
        string? city,
        string? state,
        string? postalCode,
        string? country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }
}
