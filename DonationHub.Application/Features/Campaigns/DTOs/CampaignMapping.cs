using DonationHub.Domain.Entities;

namespace DonationHub.Application.Features.Campaigns.DTOs;

public static class CampaignMapping
{
    public static AdminCampaignResponse ToAdminResponse(this Campaign campaign)
    {
        return new AdminCampaignResponse
        {
            Id = campaign.Id,
            TitleAr = campaign.TitleAr,
            TitleEn = campaign.TitleEn,
            DescriptionAr = campaign.DescriptionAr,
            DescriptionEn = campaign.DescriptionEn,
            GoalAmount = campaign.GoalAmount,
            RaisedAmount = campaign.RaisedAmount,
            EndDate = campaign.EndDate,
            CategoryId = campaign.CategoryId,
            BannerFileName = campaign.BannerFileName,
            Status = campaign.Status,
            CreatedByUserId = campaign.CreatedByUserId,
            CreatedAt = campaign.CreatedAt,
            UpdatedAt = campaign.UpdatedAt,
            RowVersion = campaign.RowVersion
        };
    }

    public static PublicCampaignResponse ToPublicResponse(
        this Campaign campaign,
        string language,
        DateTime nowUtc)
    {
        var isArabic = string.Equals(language, "ar", StringComparison.OrdinalIgnoreCase);
        var progressPercent = campaign.GoalAmount <= 0
            ? 0
            : Math.Min(100, Math.Round(campaign.RaisedAmount / campaign.GoalAmount * 100, 2));
        var daysLeft = Math.Max(0, (campaign.EndDate.Date - nowUtc.Date).Days);

        return new PublicCampaignResponse
        {
            Id = campaign.Id,
            Title = isArabic ? campaign.TitleAr : campaign.TitleEn,
            Description = isArabic ? campaign.DescriptionAr : campaign.DescriptionEn,
            GoalAmount = campaign.GoalAmount,
            RaisedAmount = campaign.RaisedAmount,
            ProgressPercent = progressPercent,
            DaysLeft = daysLeft,
            EndDate = campaign.EndDate,
            CategoryId = campaign.CategoryId,
            CategoryName = isArabic
                ? campaign.Category?.NameAr ?? string.Empty
                : campaign.Category?.NameEn ?? string.Empty,
            BannerFileName = campaign.BannerFileName,
            Status = campaign.Status,
            CreatedAt = campaign.CreatedAt
        };
    }
}
