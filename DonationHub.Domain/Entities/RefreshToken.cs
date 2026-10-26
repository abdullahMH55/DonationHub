namespace DonationHub.Domain.Entities;

public class RefreshToken : CommonData
{
    public Guid UserId { get; private set; }
    //public  TokenHash { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }
}
