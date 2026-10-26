using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities;

public class Donation : CommonData
{
    public Guid CampaignId { get; private set; }
    public Guid DonorUserId { get; private set; }
    public decimal Amount { get; private set; }
    public DonationStatus Status { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }
    public string? ReceiptNumber { get; private set; }
    public string? ReceiptFileName { get; private set; }
    public bool IsAnonymous { get; private set; }
    public DateTime? RefundedAt { get; private set; }
}
