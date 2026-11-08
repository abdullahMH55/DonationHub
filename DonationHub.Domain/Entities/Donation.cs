using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities;

public class Donation : Entity
{

    public Guid CampaignId { get; private set; }
    public Guid DonorUserId { get; private set; }
    public decimal Amount { get; private set; }
    public DonationStatus Status { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public string? FailureReason { get; private set; }
    public string ReceiptNumber { get; private set; } = string.Empty;
    public string? ReceiptFileName { get; private set; }
    public bool IsAnonymous { get; private set; }
    public DateTime? RefundedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Donation() { }

    private Donation(Guid campaignId,Guid donorUserId, decimal amount, DateTime time, bool isAnonymous = false)
    {
        CampaignId = campaignId;
        DonorUserId = donorUserId;
        Amount = amount;
        Status = DonationStatus.Pending;
        IsAnonymous = isAnonymous;
        CreatedAt = time;
    }

    public static Donation Create(Guid campaignId, Guid donorUserId, decimal amount, DateTime time, bool isAnonymous = false)
    {
        if (campaignId == Guid.Empty)
            throw new ArgumentException("CampaignId is required");
        if (donorUserId == Guid.Empty)
            throw new ArgumentException("DonorUserId is required");

        if (amount < 1.0m || amount > 10000.00m)
            throw new ArgumentException("Donation amount must1 be between 1.0 and 10000.00");

        return new Donation(campaignId, donorUserId, amount, time, isAnonymous);
    }

    public void Success(DateTime time, string receiptNumber, string? receiptFileName = null)
    {
        CheckPending();

        if (string.IsNullOrWhiteSpace(receiptNumber))
            throw new ArgumentException("Receipt number is required for succeeded donation");

        Status = DonationStatus.Succeeded;
        CompletedAt = time;
        ReceiptNumber = receiptNumber;
        ReceiptFileName = receiptFileName;
    }

    public void Fail(string reason)
    {
        CheckPending();

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Failure reason is required");

        Status = DonationStatus.Failed;
        FailureReason = reason;
    }

    private void CheckPending()
    {
        if (Status != DonationStatus.Pending)
            throw new InvalidOperationException("Donation status cannot be changed");
    }
}
