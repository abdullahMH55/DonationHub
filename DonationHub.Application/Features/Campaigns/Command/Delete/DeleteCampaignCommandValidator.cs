using FluentValidation;

namespace DonationHub.Application.Features.Campaigns.Command.Delete;

public class DeleteCampaignCommandValidator : AbstractValidator<DeleteCampaignCommand>
{
    public DeleteCampaignCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
