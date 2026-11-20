using DonationHub.Domain.Result;
using DonationHub.Domain.ResultHandling;

namespace DonationHub.Application.Common.Behaviors;

public class ValidationResult : Result, IValidationResult
{
    protected ValidationResult(Error[] errors) : base(errors)
    {
        Errors = errors;
    }
    
    public static ValidationResult WithErrors(Error[] errors) => new (errors);
    public Error[] Errors { get; }
}