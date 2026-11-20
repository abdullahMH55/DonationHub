using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using DonationHub.Domain.Errors;
using FluentAssertions;

namespace DonationHub.UnitTests.Domain;

public class CampaignTests
{
    private static readonly DateTime Now = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Create_WithZeroGoal_ReturnsGoalMustBePositive()
    {
        var result = Campaign.Create("عنوان", "Title", "وصف", "Description", 0, Now.AddDays(10), Guid.NewGuid(), Guid.NewGuid(), Now);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(CampaignDomainErrors.GoalMustBePositive);
    }

    [Fact]
    public void Create_WithPastEndDate_ReturnsEndDateInPast()
    {
        var result = Campaign.Create("عنوان", "Title", "وصف", "Description", 1000, Now.AddDays(-1), Guid.NewGuid(), Guid.NewGuid(), Now);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(CampaignDomainErrors.EndDateInPast);
    }

    [Fact]
    public void Create_SetsInitialState()
    {
        var campaign = CreateCampaign();

        campaign.Status.Should().Be(CampaignStatus.Active);
        campaign.RaisedAmount.Should().Be(0);
        campaign.CreatedAt.Should().Be(Now);
        campaign.UpdatedAt.Should().Be(Now);
        campaign.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void Update_WithGoalBelowRaisedAmount_ReturnsGoalBelowRaised()
    {
        var campaign = CreateCampaign();
        ApplyDonation(campaign, 500);

        var result = campaign.Update("عنوان", "Title", "وصف", "Description", 400, Now.AddDays(10), campaign.CategoryId, Now.AddDays(1));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(CampaignDomainErrors.GoalBelowRaised);
    }

    [Fact]
    public void Update_WhenCampaignIsCompleted_ReturnsNotActive()
    {
        var campaign = CreateCampaign();
        ApplyDonation(campaign, 1000);

        campaign.Status.Should().Be(CampaignStatus.Completed);

        var result = campaign.Update("عنوان", "Title", "وصف", "Description", 1500, Now.AddDays(10), campaign.CategoryId, Now.AddDays(1));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(CampaignDomainErrors.NotActive);
    }

    [Fact]
    public void ApplyRaisedAmount_WhenGoalReached_CompletesCampaign()
    {
        var campaign = CreateCampaign();

        ApplyDonation(campaign, 1000);

        campaign.Status.Should().Be(CampaignStatus.Completed);
        campaign.RaisedAmount.Should().Be(1000);
    }

    [Fact]
    public void ToPublicResponse_CapsProgressPercentAtOneHundred()
    {
        var campaign = CreateCampaign();
        ApplyDonation(campaign, 1500);

        var response = campaign.ToPublicResponse("en", Now);

        response.ProgressPercent.Should().Be(100);
    }

    [Fact]
    public void ToPublicResponse_ReturnsCorrectDaysLeftAndNeverNegative()
    {
        var campaign = CreateCampaign(endDate: Now.AddDays(5));

        var current = campaign.ToPublicResponse("en", Now);
        var afterEnd = campaign.ToPublicResponse("en", Now.AddDays(7));

        current.DaysLeft.Should().Be(5);
        afterEnd.DaysLeft.Should().Be(0);
    }

    [Fact]
    public void ToPublicResponse_SelectsLanguageFields()
    {
        var campaign = CreateCampaign();

        var english = campaign.ToPublicResponse("en", Now);
        var arabic = campaign.ToPublicResponse("ar", Now);

        english.Title.Should().Be("Title");
        arabic.Title.Should().Be("عنوان");
    }

    private static Campaign CreateCampaign(decimal goalAmount = 1000, DateTime? endDate = null)
    {
        var result = Campaign.Create(
            "عنوان",
            "Title",
            "وصف",
            "Description",
            goalAmount,
            endDate ?? Now.AddDays(10),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Now);

        return result.Value!;
    }

    private static void ApplyDonation(Campaign campaign, decimal amount)
    {
        var donation = Donation.Create(campaign.Id, Guid.NewGuid(), amount, Now);
        donation.Success(Now, $"R-{Guid.NewGuid():N}");
        campaign.ApplyRaisedAmount(donation, amount, Now);
    }
}
