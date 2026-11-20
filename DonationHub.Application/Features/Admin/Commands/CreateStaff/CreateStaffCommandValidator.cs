using FluentValidation;

namespace DonationHub.Application.Features.Admin.Commands.CreateStaff;

public class CreateStaffCommandValidator : AbstractValidator<CreateStaffCommand>
{
    public CreateStaffCommandValidator()
    {
        RuleFor(x => x.Request.UserName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Request.FullName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Request.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain an uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain a lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain a digit.");

        RuleFor(x => x.Request.Role)
            .NotEmpty()
            .Must(role => role is "Admin" or "CampaignManager")
            .WithMessage("Role must be 'Admin' or 'CampaignManager'.");
    }
}
