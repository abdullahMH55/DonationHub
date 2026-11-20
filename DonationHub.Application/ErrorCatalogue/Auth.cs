using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Domain.Enums;
using DonationHub.Domain.Result;
using DonationHub.Domain.ResultHandling;

namespace DonationHub.Application.ErrorCatalogue;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = 
        new("Auth.InvalidCredentials", "Wrong email or password.", ErrorType.Unauthorized);

    public static readonly Error EmailAlreadyExists = 
        new("Auth.EmailAlreadyExists", "Registration with an existing email.", ErrorType.Conflict);

    public static readonly Error UserNameAlreadyExists = 
        new("Auth.UserNameAlreadyExists", "Registration with an existing user name.", ErrorType.Conflict);

    public static readonly Error StaffCreationFailed = 
        new("Auth.StaffCreationFailed", "The staff account could not be created.", ErrorType.Validation);

    public static readonly Error InvalidRole = 
        new("Auth.InvalidRole", "Role must be 'Admin' or 'CampaignManager'.", ErrorType.Validation);

    public static readonly Error RefreshTokenInvalid = 
        new("Auth.RefreshTokenInvalid", "Refresh token unknown, expired or revoked.", ErrorType.Unauthorized);

    public static readonly Error AccountDeactivated = 
        new("Auth.AccountDeactivated", "Deactivated user logs in or refreshes.", ErrorType.Forbidden);

    public static readonly Error Unauthorized =
        new("Auth.Unauthorized", "Authentication is required.", ErrorType.Unauthorized);

    public static readonly Error Forbidden =
        new("Auth.Forbidden", "You do not have permission to access this resource.", ErrorType.Forbidden);
}
