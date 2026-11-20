using DonationHub.Domain.Enums;
using DonationHub.Domain.Result;
using DonationHub.Domain.ResultHandling;

namespace DonationHub.Application.ErrorCatalogue;

public static class FileErrors
{
    public static readonly Error InvalidType = 
        new("File.InvalidType", "File signature not allowed.", ErrorType.Validation);

    public static readonly Error TooLarge = 
        new("File.TooLarge", "File exceeds the maximum size.", ErrorType.PayloadTooLarge);
}

