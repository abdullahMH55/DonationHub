using FluentValidation;

namespace DonationHub.Application.Features.Campaigns.Command.Update;

public class UpdateCampaignCommandValidator : AbstractValidator<UpdateCampaignCommand>
{
    public UpdateCampaignCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Request.TitleAr)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Request.TitleEn)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Request.DescriptionAr)
            .MaximumLength(2000);

        RuleFor(x => x.Request.DescriptionEn)
            .MaximumLength(2000);

        RuleFor(x => x.Request.GoalAmount)
            .GreaterThan(0);

        RuleFor(x => x.Request.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Request.BannerFileName)
            .MaximumLength(255);

        RuleFor(x => x.Request.RowVersion)
            .NotEmpty();
    }
}
