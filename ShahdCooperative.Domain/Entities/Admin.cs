namespace ShahdCooperative.Domain.Entities;

public class Admin : BaseEntity
{
    public string ExternalAuthId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? Permissions { get; set; }
}
