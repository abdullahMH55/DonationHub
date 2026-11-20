using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Result;

public record Error(
    string Code,
    string Message,
    ErrorType Type,
    IReadOnlyDictionary<string, string[]>? Errors = null);