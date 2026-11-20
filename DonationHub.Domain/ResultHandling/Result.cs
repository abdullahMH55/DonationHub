using DonationHub.Domain.Result;

namespace DonationHub.Domain.ResultHandling;

public class Result
{
    public Error[]? Errors { get; }
    public Error? Error => Errors?.FirstOrDefault();

    protected Result()
    {
    }

    protected Result(Error[] errors)
    {
        Errors = errors;
    }

    public bool IsSuccess => Errors is null;

    public static Result Susccess() => new();
    public static Result Failure(Error error) => new(new[] { error });
    public static Result Failure(Error[] errors) => new(errors);
}
