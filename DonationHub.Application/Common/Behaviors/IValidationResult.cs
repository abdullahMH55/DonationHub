using DonationHub.Domain.Result;

namespace DonationHub.Application.Common.Behaviors;

public interface IValidationResult
{
    Error[] Errors { get; }
}