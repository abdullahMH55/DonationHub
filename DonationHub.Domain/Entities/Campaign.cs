using DonationHub.Domain.Common;
using DonationHub.Domain.Enums;
using DonationHub.Domain.Errors;
using DonationHub.Domain.ResultHandling;

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
    public byte[] RowVersion { get; private set; } = Array.Empty<byte>();
    public DateTime CreatedAt { get; private set; }
    
    
    public Category Category { get; set; } = null!;

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
        UpdatedAt = time;
        IsDeleted = false;
    }

    public static Result<Campaign> Create(string titleAr, string titleEn, string descriptionAr, string descriptionEn, decimal goalAmount, DateTime endDate, Guid categoryId, Guid createdByUserId, DateTime time, string? bannerFileName = null)
    {
        if (string.IsNullOrWhiteSpace(titleAr) || string.IsNullOrWhiteSpace(titleEn))
            return Result<Campaign>.Failure([CampaignDomainErrors.InvalidInput]);

        if (goalAmount <= 0)
            return Result<Campaign>.Failure([CampaignDomainErrors.GoalMustBePositive]);

        if (endDate <= time)
            return Result<Campaign>.Failure([CampaignDomainErrors.EndDateInPast]);

        return Result<Campaign>.Success(new Campaign(titleAr, titleEn, descriptionAr, descriptionEn, goalAmount, endDate, categoryId, createdByUserId, time, bannerFileName));
    }

    public Result<Campaign> Update(
        string titleAr,
        string titleEn,
        string descriptionAr,
        string descriptionEn,
        decimal goalAmount,
        DateTime endDate,
        Guid categoryId,
        DateTime time)
    {
        if (Status != CampaignStatus.Active)
            return Result<Campaign>.Failure([CampaignDomainErrors.NotActive]);

        if (string.IsNullOrWhiteSpace(titleAr) || string.IsNullOrWhiteSpace(titleEn))
            return Result<Campaign>.Failure([CampaignDomainErrors.InvalidInput]);

        if (goalAmount <= 0)
            return Result<Campaign>.Failure([CampaignDomainErrors.GoalMustBePositive]);

        if (endDate <= time)
            return Result<Campaign>.Failure([CampaignDomainErrors.EndDateInPast]);

        if (goalAmount < RaisedAmount)
            return Result<Campaign>.Failure([CampaignDomainErrors.GoalBelowRaised]);

        TitleAr = titleAr;
        TitleEn = titleEn;
        DescriptionAr = descriptionAr;
        DescriptionEn = descriptionEn;
        GoalAmount = goalAmount;
        EndDate = endDate;
        CategoryId = categoryId;
        UpdatedAt = time;

        return Result<Campaign>.Success(this);
    }

    public Result<Campaign> UpdateGoal(decimal newGoal, DateTime time)
    {
        if (Status != CampaignStatus.Active)
            return Result<Campaign>.Failure([CampaignDomainErrors.NotActive]);

        if (newGoal <= 0)
            return Result<Campaign>.Failure([CampaignDomainErrors.GoalMustBePositive]);

        if (newGoal < RaisedAmount)
            return Result<Campaign>.Failure([CampaignDomainErrors.GoalBelowRaised]);

        GoalAmount = newGoal;
        UpdatedAt = time;

        return Result<Campaign>.Success(this);
    }

    public Result<Campaign> UpdateEndDate(DateTime newEndDate, DateTime time)
    {
        if (Status != CampaignStatus.Active)
            return Result<Campaign>.Failure([CampaignDomainErrors.NotActive]);

        if (newEndDate <= time)
            return Result<Campaign>.Failure([CampaignDomainErrors.EndDateInPast]);

        EndDate = newEndDate;
        UpdatedAt = time;

        return Result<Campaign>.Success(this);
    }

    public bool Avaliable(DateTime time)
    {
        return Status == CampaignStatus.Active && EndDate > time;
    }

    public void ApplyRaisedAmount(Donation donation, decimal raisedAmount, DateTime time)
    {
        if (donation.Status != DonationStatus.Succeeded)
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
