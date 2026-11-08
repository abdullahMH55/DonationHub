namespace DonationHub.Infrastructure.Identity;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; } 
    public string TokenHash { get; private set; }
    public DateTime CreatedAt { get; private set; } 
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; } 
    public Guid? ReplacedByTokenId { get; private set; }

    private RefreshToken() { }

    private RefreshToken(Guid userId, string tokenHash, DateTime createdAt, DateTime expiresAt)
    {
        Id = Guid.NewGuid(); 
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public static RefreshToken Create(Guid userId, string tokenHash, DateTime createdAt, DateTime expiresAt)
    {
        return new RefreshToken(userId, tokenHash, createdAt, expiresAt);
    }

    public bool IsExpired(DateTime time)
    {
        return time >= ExpiresAt;
    }

    public bool IsRevoked()
    {
        return RevokedAt != null;
    }

    public void Revoke(DateTime time, Guid? replacedByTokenId = null)
    {
        RevokedAt = time;
        ReplacedByTokenId = replacedByTokenId;
    }
}