using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Campaigns.Command.Create;
using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Repositories;
using FluentAssertions;
using Moq;
using CampaignEntity = DonationHub.Domain.Entities.Campaign;
using CategoryEntity = DonationHub.Domain.Entities.Category;

namespace DonationHub.UnitTests.Application;

public class CreateCampaignCommandHandlerTests
{
    private static readonly DateTime Now = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);

    private readonly Mock<ICampaignRepository> _campaigns = new();
    private readonly Mock<ICategoryRepository> _categories = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly Mock<TimeProvider> _timeProvider = new();
    private readonly CreateCampaignCommandHandler _handler;

    public CreateCampaignCommandHandlerTests()
    {
        _timeProvider.Setup(provider => provider.GetUtcNow()).Returns(new DateTimeOffset(Now));
        _handler = new CreateCampaignCommandHandler(
            _campaigns.Object,
            _categories.Object,
            _unitOfWork.Object,
            _timeProvider.Object);
    }

    [Fact]
    public async Task Handle_WhenCategoryDoesNotExist_ReturnsCategoryNotFoundAndDoesNotSave()
    {
        _categories
            .Setup(repository => repository.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((CategoryEntity?)null);

        var result = await _handler.Handle(CreateCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(CategoryErrors.NotFound);
        _campaigns.Verify(
            repository => repository.AddAsync(It.IsAny<CampaignEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
        _unitOfWork.Verify(
            unitOfWork => unitOfWork.CompleteAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_WhenValid_AddsCampaignAndSavesOnce()
    {
        var category = CategoryEntity.Create("فئة", "Category", "وصف", "Description", Now);
        _categories
            .Setup(repository => repository.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        _campaigns
            .Setup(repository => repository.AddAsync(It.IsAny<CampaignEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _unitOfWork
            .Setup(unitOfWork => unitOfWork.CompleteAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var command = CreateCommand(category.Id);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.TitleEn.Should().Be("Title");
        result.Value.CategoryId.Should().Be(category.Id);
        _campaigns.Verify(
            repository => repository.AddAsync(
                It.Is<CampaignEntity>(campaign => campaign.TitleEn == "Title"),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWork.Verify(
            unitOfWork => unitOfWork.CompleteAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static CreateCampaignCommand CreateCommand(Guid? categoryId = null)
    {
        return new CreateCampaignCommand(
            new CreateCampaignRequest
            {
                TitleAr = "عنوان",
                TitleEn = "Title",
                DescriptionAr = "وصف",
                DescriptionEn = "Description",
                GoalAmount = 1000,
                EndDate = Now.AddDays(10),
                CategoryId = categoryId ?? Guid.NewGuid()
            },
            Guid.NewGuid());
    }
}
