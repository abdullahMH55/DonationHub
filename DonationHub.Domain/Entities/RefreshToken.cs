using DonationHub.Domain.Common;
using DonationHub.Domain.Errors;
using DonationHub.Domain.ResultHandling;

namespace DonationHub.Domain.Entities;

public class RefreshToken : Entity
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public Guid? ReplacedByTokenId { get; private set; }

    private RefreshToken() { }

    private RefreshToken(Guid userId, string tokenHash, DateTime createdAt, DateTime expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }

    public static Result<RefreshToken> Create(Guid userId, string tokenHash, DateTime createdAt, DateTime expiresAt)
    {
        if (userId == Guid.Empty)
            return Result<RefreshToken>.Failure([RefreshTokenDomainErrors.UserIdRequired]);

        if (string.IsNullOrWhiteSpace(tokenHash))
            return Result<RefreshToken>.Failure([RefreshTokenDomainErrors.TokenHashRequired]);

        if (expiresAt <= createdAt)
            return Result<RefreshToken>.Failure([RefreshTokenDomainErrors.InvalidExpiry]);

        return Result<RefreshToken>.Success(new RefreshToken(userId, tokenHash, createdAt, expiresAt));
    }

    public bool IsExpired(DateTime time)
    {
        return time >= ExpiresAt;
    }

    public bool IsRevoked()
    {
        return RevokedAt is not null;
    }

    public bool IsActive(DateTime time)
    {
        return !IsRevoked() && !IsExpired(time);
    }

    public void Revoke(DateTime time, Guid? replacedByTokenId = null)
    {
        if (IsRevoked())
            return;

        RevokedAt = time;
        ReplacedByTokenId = replacedByTokenId;
    }
}
