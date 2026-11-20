using DonationHub.Domain.Enums;
using DonationHub.Domain.Result;
using DonationHub.Domain.ResultHandling;

namespace DonationHub.Application.ErrorCatalogue;

public static class PaymentErrors
{
    public static readonly Error InvalidSignature = 
        new("Payment.InvalidSignature", "Stripe webhook signature invalid.", ErrorType.Validation);

    public static readonly Error KeyReusedWithDifferentRequest = 
        new("Idempotency.KeyReusedWithDifferentRequest", "Same Idempotency-Key with a different body.", ErrorType.UnprocessableEntity);
}