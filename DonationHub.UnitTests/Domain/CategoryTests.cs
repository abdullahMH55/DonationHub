using DonationHub.Domain.Entities;
using DonationHub.Domain.Errors;
using FluentAssertions;

namespace DonationHub.UnitTests.Domain;

public class CategoryTests
{
    private static readonly DateTime Now = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void EnsureCanBeDeleted_WhenCategoryHasCampaigns_ReturnsHasCampaigns()
    {
        var category = Category.Create("عنوان", "Title", "وصف", "Description", Now);

        var result = category.EnsureCanBeDeleted(true);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(CategoryDomainErrors.HasCampaigns);
    }

    [Fact]
    public void EnsureCanBeDeleted_WhenCategoryHasNoCampaigns_Succeeds()
    {
        var category = Category.Create("عنوان", "Title", "وصف", "Description", Now);

        var result = category.EnsureCanBeDeleted(false);

        result.IsSuccess.Should().BeTrue();
    }
}
