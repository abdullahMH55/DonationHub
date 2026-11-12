using DonationHub.Application.Features.Categories.DTOs;
using DonationHub.Application.Repositories;

namespace DonationHub.Application.Features.Categories.GetAll;

public class GetAllCategoriesHandler
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryResponse>> HandleAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories.Select(c => new CategoryResponse
        {
            Id = c.Id,
            NameAr = c.NameAr,
            NameEn = c.NameEn,
            DescriptionAr = c.DescriptionAr,
            DescriptionEn = c.DescriptionEn,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();
    }
}
