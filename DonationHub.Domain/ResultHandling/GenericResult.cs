using DonationHub.Domain.Result;

namespace DonationHub.Domain.ResultHandling;

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T value)
    {
        Value = value;
    }

    protected Result(Error[] errors) : base(errors)
    {
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(new[] { error });
    public static Result<T> Failure(Error[] errors) => new(errors);
}
