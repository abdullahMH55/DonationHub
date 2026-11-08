using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Value_Objects;

public record Money(decimal  Amount, Currency? Currency);