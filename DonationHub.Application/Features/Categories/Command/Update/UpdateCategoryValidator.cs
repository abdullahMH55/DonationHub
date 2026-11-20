using FluentValidation;

namespace DonationHub.Application.Features.Categories.Command.Update;

public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Request.NameAr)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Request.NameEn)
            .NotEmpty()
            .MaximumLength(100);

    }
}
