using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Value_Objects;

public record Money(int  Amount, Currency? Currency);