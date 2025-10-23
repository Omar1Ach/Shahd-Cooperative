namespace ShahdCooperative.Application.Common.Interfaces;

/// <summary>
/// Service to access the current authenticated user's information from JWT claims
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID from the JWT token claims
    /// </summary>
    string? UserId { get; }

    /// <summary>
    /// Gets the current user's email from the JWT token claims
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Gets the current user's roles from the JWT token claims
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Checks if the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    bool IsInRole(string role);
}
