using DonationHub.Domain.Enums;
using DonationHub.Domain.Result;

namespace DonationHub.Domain.Errors;

public static class CategoryDomainErrors
{
    public static readonly Error HasCampaigns =
        new("Category.HasCampaigns", "Cannot delete a category with linked campaigns.", ErrorType.Conflict);
}
