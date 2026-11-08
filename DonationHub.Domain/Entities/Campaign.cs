using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;

namespace DonationHub.Domain.Entities;

public class Campaign : Entity, ISoftDeletable
{
    public string TitleAr { get; private set; } = string.Empty;
    public string TitleEn { get; private set; } = string.Empty;
    public string DescriptionAr { get; private set; } = string.Empty;
    public string DescriptionEn { get; private set; } = string.Empty;
    public decimal GoalAmount { get; private set; }
    public decimal RaisedAmount { get; private set; }
    public DateTime EndDate { get; private set; }
    public Guid CategoryId { get; private set; }
    public string? BannerFileName { get; private set; }
    public CampaignStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public byte[] RowVersion { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    
    public Category Category { get; set; }

    private Campaign() { }

    private Campaign(string titleAr, string titleEn, string descriptionAr, string descriptionEn, decimal goalAmount, DateTime endDate, Guid categoryId, Guid createdByUserId, DateTime time, string? bannerFileName = null)
    {
        TitleAr = titleAr;
        TitleEn = titleEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        GoalAmount = goalAmount;
        RaisedAmount = 0;
        EndDate = endDate;
        CategoryId = categoryId;
        CreatedByUserId = createdByUserId;
        BannerFileName = bannerFileName;
        Status = CampaignStatus.Active;
        CreatedAt = time;
        IsDeleted = false;
    }

    public static Campaign Create(string titleAr, string titleEn, string descriptionAr, string descriptionEn, decimal goalAmount, DateTime endDate, Guid categoryId, Guid createdByUserId, DateTime time, string? bannerFileName = null)
    {
        if (string.IsNullOrWhiteSpace(titleAr))
            throw new ArgumentException("Title in Arabic is required");
        if (string.IsNullOrWhiteSpace(titleEn))
            throw new ArgumentException("Title in English is required");
        
        if (goalAmount <= 0)
            throw new ArgumentException("Goal amount must be greater than zero");
        
        if (endDate <= time)
            throw new ArgumentException("End date must be in the future");

        return new Campaign(titleAr, titleEn, descriptionAr, descriptionEn, goalAmount, endDate, categoryId, createdByUserId, time, bannerFileName);
    }
    
    public void UpdateGoal(decimal newGoal, DateTime time)
    {
        CheckAvaliable();

        if (newGoal <= 0)
            throw new ArgumentException("Goal amount must be greater than zero");

        if (newGoal < RaisedAmount)
            throw new InvalidOperationException("Goal amount cannot be lowered below the current raised amount");

        GoalAmount = newGoal;
        UpdatedAt = time;
    }
    
    public void UpdateEndDate(DateTime newEndDate, DateTime time)
    {
        CheckAvaliable();

        if (newEndDate <= time)
            throw new ArgumentException("End date must be in the future");

        EndDate = newEndDate;
        UpdatedAt = time;
    }

    public bool Avaliable(DateTime time)
    {
        return Status == CampaignStatus.Active && EndDate > time;
    }

    public void ApplyRaisedAmount(Donation donation ,decimal raisedAmount, DateTime time)
    {
        if(donation.Status != DonationStatus.Succeeded)
            throw new ArgumentException("The donation process not successful");
        
        if (!Avaliable(time))
            throw new InvalidOperationException("Campaign cannot accept donations because it is not active or has expired");
            
        if (raisedAmount <= 0)
            throw new ArgumentException("Donation amount must be greater than zero");


        RaisedAmount += raisedAmount;
        UpdatedAt = time;
        
        if (RaisedAmount >= GoalAmount)
        {
            Status = CampaignStatus.Completed;
        }
    }

    public void Expire(DateTime time)
    {
        CheckAvaliable();

        if (time < EndDate)
            throw new InvalidOperationException("Campaign not reached its end date yet");

        Status = CampaignStatus.Expired;
        UpdatedAt = time;
    }
    
    public void SoftDelete(DateTime time)
    {
        IsDeleted = true;
        DeletedAtUtc = time;
        UpdatedAt = time;
    }

    private void CheckAvaliable()
    {
        if (Status == CampaignStatus.Completed || Status == CampaignStatus.Expired)
            throw new InvalidOperationException($"Campaign is in a terminal state ({Status}) and cannot be modified");
    }
}
