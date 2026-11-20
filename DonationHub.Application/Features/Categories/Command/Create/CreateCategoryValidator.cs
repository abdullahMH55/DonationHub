using FluentValidation;

namespace DonationHub.Application.Features.Categories.Command.Create;

public class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Request.NameAr)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Request.NameEn)
            .NotEmpty()
            .MaximumLength(100);

    }
}
