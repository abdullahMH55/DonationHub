using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Categories.Command.Delete;
using DonationHub.Application.Repositories;
using FluentAssertions;
using Moq;
using CategoryEntity = DonationHub.Domain.Entities.Category;

namespace DonationHub.UnitTests.Application;

public class CategoryHandlerTests
{
    private static readonly DateTime Now = new(2026, 11, 10, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task DeleteCategory_WhenCategoryHasCampaigns_ReturnsHasCampaigns()
    {
        var category = CategoryEntity.Create("فئة", "Category", "وصف", "Description", Now);
        var categories = new Mock<ICategoryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();

        categories
            .Setup(repository => repository.GetByIdAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);
        categories
            .Setup(repository => repository.HasCampaignsAsync(category.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new DeleteCategoryHandler(categories.Object, unitOfWork.Object);
        var result = await handler.Handle(new DeleteCategoryCommand(category.Id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be(CategoryErrors.HasCampaigns);
        categories.Verify(repository => repository.Delete(It.IsAny<CategoryEntity>()), Times.Never);
        unitOfWork.Verify(work => work.CompleteAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
