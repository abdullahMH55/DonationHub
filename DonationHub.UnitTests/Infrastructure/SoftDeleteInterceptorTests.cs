using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Data;
using DonationHub.Infrastructure.Interceptors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;

namespace DonationHub.UnitTests.Infrastructure;

public class SoftDeleteInterceptorTests
{
    [Fact]
    public void SavingChanges_WhenEntityIsSoftDeletable_MarksItAsDeleted()
    {
        using var context = CreateContext();
        var campaign = CreateCampaign();
        TrackAsDeleted(context, campaign);

        var interceptor = new SoftDeleteInterceptor(TimeProvider.System);
        interceptor.SavingChanges(CreateEventData(context), default);

        campaign.IsDeleted.Should().BeTrue();
        campaign.DeletedAtUtc.Should().NotBeNull();
        context.Entry(campaign).State.Should().Be(EntityState.Modified);
    }

    [Fact]
    public async Task SavingChangesAsync_WhenEntityIsSoftDeletable_MarksItAsDeleted()
    {
        using var context = CreateContext();
        var campaign = CreateCampaign();
        TrackAsDeleted(context, campaign);

        var interceptor = new SoftDeleteInterceptor(TimeProvider.System);
        await interceptor.SavingChangesAsync(CreateEventData(context), default, CancellationToken.None);

        campaign.IsDeleted.Should().BeTrue();
        campaign.DeletedAtUtc.Should().NotBeNull();
        context.Entry(campaign).State.Should().Be(EntityState.Modified);
    }

    [Fact]
    public void SavingChanges_WhenEntityIsNotDeleted_LeavesItUntouched()
    {
        using var context = CreateContext();
        var campaign = CreateCampaign();
        context.Campaigns.Attach(campaign);

        var interceptor = new SoftDeleteInterceptor(TimeProvider.System);
        interceptor.SavingChanges(CreateEventData(context), default);

        campaign.IsDeleted.Should().BeFalse();
        campaign.DeletedAtUtc.Should().BeNull();
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DonationHubUnitTests;Trusted_Connection=True;")
            .Options;

        return new AppDbContext(options);
    }

    private static Campaign CreateCampaign()
    {
        var nowUtc = new DateTime(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);

        return Campaign.Create(
            "عنوان",
            "Title",
            "وصف",
            "Description",
            1000,
            nowUtc.AddDays(10),
            Guid.NewGuid(),
            Guid.NewGuid(),
            nowUtc).Value!;
    }

    private static void TrackAsDeleted(AppDbContext context, Campaign campaign)
    {
        context.Campaigns.Attach(campaign);
        context.Entry(campaign).State = EntityState.Deleted;
    }

    private static DbContextEventData CreateEventData(AppDbContext context)
    {
        return new DbContextEventData(new TestEventDefinition(), (_, _) => "test", context);
    }

    private sealed class TestEventDefinition : EventDefinitionBase
    {
        public TestEventDefinition()
            : base(
                Mock.Of<ILoggingOptions>(),
                new EventId(1, "TestEvent"),
                LogLevel.Information,
                "TestEvent")
        {
        }
    }
}
