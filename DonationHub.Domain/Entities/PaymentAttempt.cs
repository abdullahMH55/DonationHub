using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities;

public class PaymentAttempt : CommonData
{
    public Guid DonationId { get; private set; }
    //public  StripeCheckoutSessionId { get; private set; }
    //public  StripePaymentIntentId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentAttemptStatus Status { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string? FailureCode { get; private set; }
}
