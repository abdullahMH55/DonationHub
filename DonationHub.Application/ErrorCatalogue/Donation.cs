using DonationHub.Domain.Enums;
using DonationHub.Domain.Result;
using DonationHub.Domain.ResultHandling;

namespace DonationHub.Application.ErrorCatalogue;

public static class DonationErrors
{
    public static readonly Error NotFound = 
        new("Donation.NotFound", "Donation not found or not owned.", ErrorType.NotFound);

    public static readonly Error AmountOutOfRange = 
        new("Donation.AmountOutOfRange", "Amount outside the DonationOptions range.", ErrorType.UnprocessableEntity);

    public static readonly Error InvalidState = 
        new("Donation.InvalidState", "Donation already in a final state.", ErrorType.Conflict);
}

