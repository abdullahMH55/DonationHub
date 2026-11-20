using FluentValidation;

namespace DonationHub.Application.Features.Campaigns.Query.GetById;

public class GetCampaignByIdQueryValidator : AbstractValidator<GetCampaignByIdQuery>
{
    public GetCampaignByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Language)
            .Must(language => language is "en" or "ar")
            .WithMessage("Language must be 'en' or 'ar'.");
    }
}
