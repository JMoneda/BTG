namespace BTG.Domain.Entities;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "Cliente"; // Admin | Cliente
    public List<RefreshToken> RefreshTokens { get; set; } = new();
}

