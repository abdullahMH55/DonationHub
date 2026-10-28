using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities;

public class PaymentAttempt : Enitiy
{
    public Guid DonationId { get; private set; }
    public string StripeCheckoutSessionId { get; private set; } = string.Empty;
    public string? StripePaymentIntentId { get; private set;}
    public decimal Amount { get; private set; }
    public PaymentAttemptStatus Status { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string? FailureCode { get; private set; }
    public DateTime CreatedAt { get; private set; }
}
