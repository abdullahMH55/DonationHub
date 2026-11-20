using DonationHub.Domain.Enums;

namespace DonationHub.Application.Features.Campaigns.DTOs;

public class CreateCampaignRequest
{
    public Guid UserId { get; set; } 
    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public decimal GoalAmount { get; set; }
    public DateTime EndDate { get; set; }
    public Guid CategoryId { get; set; }
    public string? BannerFileName { get; set; }
}

public class UpdateCampaignRequest
{
    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public decimal GoalAmount { get; set; }
    public DateTime EndDate { get; set; }
    public Guid CategoryId { get; set; }
    public string? BannerFileName { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

public class AdminCampaignResponse
{
    public Guid Id { get; set; }
    public string TitleAr { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public decimal GoalAmount { get; set; }
    public decimal RaisedAmount { get; set; }
    public DateTime EndDate { get; set; }
    public Guid CategoryId { get; set; }
    public string? BannerFileName { get; set; }
    public CampaignStatus Status { get; set; }
    public Guid CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();
}

public class PublicCampaignResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal GoalAmount { get; set; }
    public decimal RaisedAmount { get; set; }
    public decimal ProgressPercent { get; set; }
    public int DaysLeft { get; set; }
    public DateTime EndDate { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string? BannerFileName { get; set; }
    public CampaignStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
