using DonationHub.Domain.Enums;
using DonationHub.Domain.Result;

namespace DonationHub.Domain.Errors;

public static class CampaignDomainErrors
{
    public static readonly Error InvalidInput =
        new("Validation.Failed", "Input validation failure.", ErrorType.Validation);

    public static readonly Error GoalMustBePositive =
        new("Validation.Failed", "Goal amount must be greater than zero.", ErrorType.Validation);

    public static readonly Error EndDateInPast =
        new("Campaign.EndDateInPast", "End date must be in the future.", ErrorType.UnprocessableEntity);

    public static readonly Error GoalBelowRaised =
        new("Campaign.GoalBelowRaised", "Goal cannot be lower than the raised amount.", ErrorType.UnprocessableEntity);

    public static readonly Error NotActive =
        new("Campaign.NotActive", "Campaign is completed or expired.", ErrorType.Conflict);
}
