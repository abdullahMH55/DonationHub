using DonationHub.Domain.Enums;
using DonationHub.Domain.Errors;
using DonationHub.Domain.Result;
using DonationHub.Domain.ResultHandling;

namespace DonationHub.Application.ErrorCatalogue;

public static class CategoryErrors
{
    public static readonly Error NotFound = 
        new("Category.NotFound", "Category does not exist.", ErrorType.NotFound);

    public static readonly Error HasCampaigns = CategoryDomainErrors.HasCampaigns;
    
    public static readonly Error AlreadyExists = 
        new("Category.AlreadyExists", "Category already exist.", ErrorType.Conflict);
}


