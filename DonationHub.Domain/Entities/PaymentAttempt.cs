using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities;

public class PaymentAttempt : Entity
{
    public Guid DonationId { get; private set; }
    public string StripeCheckoutSessionId { get; private set; } = string.Empty;
    public string? StripePaymentIntentId { get; private set;}
    public decimal Amount { get; private set; }
    public PaymentAttemptStatus Status { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public string? FailureCode { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private PaymentAttempt() { }

    private PaymentAttempt(Guid donationId, string stripeCheckoutSessionId, string? stripePaymentIntentId, decimal amount, DateTime createdAt)
    {
        DonationId = donationId;
        StripeCheckoutSessionId = stripeCheckoutSessionId;
        StripePaymentIntentId = stripePaymentIntentId;
        Amount = amount;
        Status = PaymentAttemptStatus.Created;
        CreatedAt = createdAt;

    }

    public static PaymentAttempt Create(Guid donationId, string stripeCheckoutSessionId, string? stripePaymentIntentId, decimal amount, DateTime createdAt)
    {
        if (amount < 1.0m || amount > 10000.00m)
            throw new ArgumentOutOfRangeException(nameof(amount), $"Donation amount must1 be between 1.0 and 10000.00");
        return new PaymentAttempt(donationId, stripeCheckoutSessionId, stripePaymentIntentId, amount, createdAt);
    }

    public void Success(DateTime time)
    {
        CheckFinalState();
        Status = PaymentAttemptStatus.Succeeded;
        UpdatedAt = time;
    }
    
    public void Fail(string failureCode, DateTime time)
    {
        CheckFinalState();
        if(string.IsNullOrEmpty(failureCode))
            throw new ArgumentException("FailureCode reason is required");
        
        FailureCode = failureCode;
        Status = PaymentAttemptStatus.Failed;
        UpdatedAt =  time;
        
    }
    public void Expired(DateTime time)
    {
        CheckFinalState();
        Status = PaymentAttemptStatus.Expired;
        UpdatedAt = time;
        
    }

    public void CheckFinalState()
    {
        if(Status != PaymentAttemptStatus.Created)
            throw new InvalidOperationException("Status cannot be changed");

    }
}
