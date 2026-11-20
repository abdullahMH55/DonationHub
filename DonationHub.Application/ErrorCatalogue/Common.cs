using DonationHub.Domain.Enums;
using DonationHub.Domain.Result;
using DonationHub.Domain.ResultHandling;

namespace DonationHub.Application.ErrorCatalogue;

public static class CommonErrors
{
    public static readonly Error ValidationFailed = 
        new("Validation.Failed", "Input validation failure.", ErrorType.Validation);

    public static readonly Error RateLimited = 
        new("Common.RateLimited", "Rate limit exceeded.", ErrorType.TooManyRequests);

    public static readonly Error UnexpectedError =
        new("Common.UnexpectedError", "An unexpected error occurred.", ErrorType.Failure);

    public static readonly Error NotFound =
        new("Common.NotFound", "The requested resource was not found.", ErrorType.NotFound);

    public static readonly Error MethodNotAllowed =
        new("Common.MethodNotAllowed", "The HTTP method is not allowed for this resource.", ErrorType.MethodNotAllowed);

    public static readonly Error UnsupportedMediaType =
        new("Common.UnsupportedMediaType", "The request media type is not supported.", ErrorType.UnsupportedMediaType);
}
