namespace DonationHub.Domain.Entities;

public class ProcessedStripeEvent
{
    public string EventId { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public DateTime ProcessedAt { get; private set; }
}
