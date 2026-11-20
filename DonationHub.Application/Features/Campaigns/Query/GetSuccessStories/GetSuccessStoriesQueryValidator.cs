using FluentValidation;

namespace DonationHub.Application.Features.Campaigns.Query.GetSuccessStories;

public class GetSuccessStoriesQueryValidator : AbstractValidator<GetSuccessStoriesQuery>
{
    public GetSuccessStoriesQueryValidator()
    {
        RuleFor(x => x.Language)
            .Must(language => language is "en" or "ar")
            .WithMessage("Language must be 'en' or 'ar'.");
    }
}
