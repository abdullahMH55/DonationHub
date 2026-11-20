using DonationHub.Application.Common.Exceptions;
using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Campaigns.Command.Update;
using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Repositories;
using FluentAssertions;
using Moq;
using CampaignEntity = DonationHub.Domain.Entities.Campaign;
using CategoryEntity = DonationHub.Domain.Entities.Category;

namespace DonationHub.UnitTests.Application;

public class UpdateCampaignCommandHandlerTests
{
    private static readonly DateTime Now = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);

    private readonly Mock<ICampaignRepository> _campaigns = new();
    private readonly Mock<ICategoryRepository> _categories = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<TimeProvider> _timeProvider = new();
    private readonly UpdateCampaignCommandHandler _handler;

    public UpdateCampaignCommandHandlerTests()
    {
        _timeProvider.Setup(provider => provider.GetUtcNow()).Returns(new DateTimeOffset(Now));
        _handler = new UpdateCampaignCommandHandler(
            _campaigns.Object,
            _categories.Object,
            _unitOfWork.Object,
            _timeProvider.Object);
    }

    [Fact]
    public async Task Handle_WhenCampaignDoesNotExist_ReturnsCampaignNotFound()
    {
        _campaigns
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CampaignEntity?)null);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(CampaignErrors.NotFound);
    }

    [Fact]
    public async Task Handle_WhenUnitOfWorkThrowsConcurrencyConflict_ReturnsConcurrencyConflict()
    {
        var campaign = CreateCampaign();
        var category = CategoryEntity.Create("فئة", "Category", "وصف", "Description", Now);

        _campaigns
            .Setup(repository => repository.GetByIdAsync(campaign.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(campaign);
        _categories
            .Setup(repository => repository.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _unitOfWork
            .Setup(unitOfWork => unitOfWork.CompleteAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConcurrencyConflictException());

        var command = CreateCommand(campaign.Id, category.Id, campaign.RowVersion);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(CampaignErrors.ConcurrencyConflict);
    }

    private static CampaignEntity CreateCampaign()
    {
        var result = CampaignEntity.Create(
            "عنوان",
            "Title",
            "وصف",
            "Description",
            1000,
            Now.AddDays(10),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Now);

        return result.Value!;
    }

    private static UpdateCampaignCommand CreateCommand(
        Guid? id = null,
        Guid? categoryId = null,
        byte[]? rowVersion = null)
    {
        return new UpdateCampaignCommand(
            id ?? Guid.NewGuid(),
            new UpdateCampaignRequest
            {
                TitleAr = "عنوان محدث",
                TitleEn = "Updated title",
                DescriptionAr = "وصف",
                DescriptionEn = "Description",
                GoalAmount = 1200,
                EndDate = Now.AddDays(15),
                CategoryId = categoryId ?? Guid.NewGuid(),
                RowVersion = rowVersion ?? new byte[] { 1, 2, 3 }
            });
    }
}
