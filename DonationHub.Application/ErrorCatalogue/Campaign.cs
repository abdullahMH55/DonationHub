using DonationHub.Domain.Enums;
using DonationHub.Domain.Result;
using DonationHub.Domain.ResultHandling;

namespace DonationHub.Application.ErrorCatalogue;

public static class CampaignErrors
{
    public static readonly Error NotFound = 
        new("Campaign.NotFound", "Campaign ID not found.", ErrorType.NotFound);

    public static readonly Error NotActive = 
        new("Campaign.NotActive", "Campaign is completed or expired.", ErrorType.Conflict);

    public static readonly Error GoalBelowRaised = 
        new("Campaign.GoalBelowRaised", "Goal cannot be lower than the raised amount.", ErrorType.UnprocessableEntity);

    public static readonly Error EndDateInPast = 
        new("Campaign.EndDateInPast", "End date must be in the future.", ErrorType.UnprocessableEntity);

    public static readonly Error ConcurrencyConflict = 
        new("Campaign.ConcurrencyConflict", "Concurrent update detected.", ErrorType.Conflict);
}


