namespace VaultCode.Models;

public abstract class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string Initials => string.Concat(
        string.IsNullOrEmpty(FirstName) ? "" : FirstName[..1],
        string.IsNullOrEmpty(LastName) ? "" : LastName[..1]
    ).ToUpperInvariant();
    public string Role { get; set; } = "Agent";
}
