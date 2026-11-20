using FluentValidation;

namespace DonationHub.Application.Features.Campaigns.Query.GetAll;

public class GetCampaignsQueryValidator : AbstractValidator<GetCampaignsQuery>
{
    public GetCampaignsQueryValidator()
    {
        RuleFor(x => x.Language)
            .Must(language => language is "en" or "ar")
            .WithMessage("Language must be 'en' or 'ar'.");
    }
}
