namespace DonationHub.Domain.Enums;

public enum ErrorType
{
    Validation = 400,
    Unauthorized = 401,
    Forbidden = 403, 
    NotFound = 404,
    MethodNotAllowed = 405,
    Conflict = 409,
    PayloadTooLarge = 413,
    UnsupportedMediaType = 415,
    UnprocessableEntity = 422,
    TooManyRequests = 429,
    Failure = 500
}
