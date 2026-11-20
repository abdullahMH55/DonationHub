using System.Reflection;
using DonationHub.Application.ErrorCatalogue;
using DonationHub.Domain.Result;
using DonationHub.Domain.ResultHandling;
using FluentValidation;
using MediatR;

namespace DonationHub.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : Result
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var errors = failures
            .GroupBy(failure => failure.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(failure => failure.ErrorMessage).ToArray());

        var error = new Error(
            CommonErrors.ValidationFailed.Code,
            CommonErrors.ValidationFailed.Message,
            CommonErrors.ValidationFailed.Type,
            errors);

        return CreateFailure(error);
    }

    private static TResponse CreateFailure(Error error)
    {
        if (typeof(TResponse) == typeof(Result))
            return (TResponse)(object)Result.Failure(error);

        var resultType = typeof(TResponse);
        if (resultType.IsGenericType &&
            resultType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var failureMethod = resultType.GetMethod(
                nameof(Result<object>.Failure),
                new[] { typeof(Error) });

            if (failureMethod is not null)
                return (TResponse)failureMethod.Invoke(null, new object[] { error })!;
        }

        throw new InvalidOperationException(
            $"ValidationBehavior only supports Result and Result<T> responses. Got {resultType.Name}.");
    }
}
