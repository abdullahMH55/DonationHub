using FluentValidation;

namespace DonationHub.Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
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

        RuleFor(x => x.Request.PreferredLanguage)
            .NotEmpty()
            .Must(language => language is not null &&
                (language.Equals("en", StringComparison.OrdinalIgnoreCase) ||
                 language.Equals("ar", StringComparison.OrdinalIgnoreCase)))
            .WithMessage("Preferred language must be 'en' or 'ar'.");
    }
}
