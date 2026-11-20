using DonationHub.Domain.Entities;
using DonationHub.Domain.Errors;
using FluentAssertions;

namespace DonationHub.UnitTests.Domain;

public class RefreshTokenTests
{
    private static readonly DateTime Now = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_WhenUserIdIsEmpty_ReturnsUserIdRequired()
    {
        var result = RefreshToken.Create(Guid.Empty, "hash", Now, Now.AddDays(7));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(RefreshTokenDomainErrors.UserIdRequired);
    }

    [Fact]
    public void Create_WhenTokenHashIsEmpty_ReturnsTokenHashRequired()
    {
        var result = RefreshToken.Create(Guid.NewGuid(), "  ", Now, Now.AddDays(7));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(RefreshTokenDomainErrors.TokenHashRequired);
    }

    [Fact]
    public void Create_WhenExpiryIsNotAfterCreation_ReturnsInvalidExpiry()
    {
        var result = RefreshToken.Create(Guid.NewGuid(), "hash", Now, Now);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(RefreshTokenDomainErrors.InvalidExpiry);
    }

    [Fact]
    public void Create_WithValidData_SetsPropertiesAndStartsActive()
    {
        var userId = Guid.NewGuid();

        var result = RefreshToken.Create(userId, "hash", Now, Now.AddDays(7));

        result.IsSuccess.Should().BeTrue();
        result.Value!.UserId.Should().Be(userId);
        result.Value.TokenHash.Should().Be("hash");
        result.Value.CreatedAt.Should().Be(Now);
        result.Value.ExpiresAt.Should().Be(Now.AddDays(7));
        result.Value.RevokedAt.Should().BeNull();
        result.Value.IsRevoked().Should().BeFalse();
        result.Value.IsExpired(Now.AddDays(8)).Should().BeTrue();
        result.Value.IsActive(Now).Should().BeTrue();
    }

    [Fact]
    public void Revoke_SetsRevokedAtAndReplacementToken()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "hash", Now, Now.AddDays(7)).Value!;
        var replacementId = Guid.NewGuid();

        token.Revoke(Now.AddHours(1), replacementId);

        token.RevokedAt.Should().Be(Now.AddHours(1));
        token.ReplacedByTokenId.Should().Be(replacementId);
        token.IsRevoked().Should().BeTrue();
        token.IsActive(Now).Should().BeFalse();
    }

    [Fact]
    public void Revoke_WhenAlreadyRevoked_KeepsTheFirstRevocation()
    {
        var token = RefreshToken.Create(Guid.NewGuid(), "hash", Now, Now.AddDays(7)).Value!;

        token.Revoke(Now.AddHours(1));
        token.Revoke(Now.AddHours(5));

        token.RevokedAt.Should().Be(Now.AddHours(1));
        token.ReplacedByTokenId.Should().BeNull();
    }
}
