using DonationHub.Domain.Result;
using DonationHub.Domain.ResultHandling;

namespace DonationHub.Application.Common.Behaviors;

public class ValidationResult<TValue> : Result<TValue>, IValidationResult
{
    protected ValidationResult(Error[] errors) : base(errors)
    {
        Errors = errors;
    }
    
    public Error[] Errors { get; }

    public static ValidationResult<TValue> WithErrors(Error[] errors) => new(errors);

}