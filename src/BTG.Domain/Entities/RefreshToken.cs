namespace BTG.Domain.Entities
{
    public class RefreshToken
    {
        public string Token { get; set; } = Guid.NewGuid().ToString();
        public DateTime ExpiresAtUtc { get; set; } = DateTime.UtcNow.AddDays(7); // configurable
        public bool Revoked { get; set; } = false;
        public string? ReplacedByToken { get; set; }

        public bool IsActive => !Revoked && DateTime.UtcNow <= ExpiresAtUtc;
    }
}
