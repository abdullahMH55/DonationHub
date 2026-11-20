using DonationHub.Domain.Enums;
using DonationHub.Domain.Result;

namespace DonationHub.Domain.Errors;

public static class RefreshTokenDomainErrors
{
    public static readonly Error UserIdRequired =
        new("Auth.RefreshTokenUserRequired", "A refresh token must belong to a user.", ErrorType.Validation);

    public static readonly Error TokenHashRequired =
        new("Auth.RefreshTokenHashRequired", "A refresh token must have a hash.", ErrorType.Validation);

    public static readonly Error InvalidExpiry =
        new("Auth.RefreshTokenInvalidExpiry", "A refresh token must expire after it was created.", ErrorType.Validation);
}
