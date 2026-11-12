using DonationHub.Application.Features.Categories.DTOs;
using DonationHub.Application.Repositories;
using DonationHub.Domain.Entities;

namespace DonationHub.Application.Features.Categories.Create;

public class CreateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponse?> HandleAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var existsAr = await _categoryRepository.ExistsArNameAsync(request.NameAr, cancellationToken);
        var existsEn = await _categoryRepository.ExistsEnNameAsync(request.NameEn, cancellationToken);
        if (existsAr || existsEn)
        {
            return null;
        }

        var category = Category.Create(
            request.NameAr,
            request.NameEn,
            request.DescriptionAr ?? string.Empty,
            request.DescriptionEn ?? string.Empty,
            DateTime.UtcNow
        );

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return new CategoryResponse
        {
            Id = category.Id,
            NameAr = category.NameAr,
            NameEn = category.NameEn,
            DescriptionAr = category.DescriptionAr,
            DescriptionEn = category.DescriptionEn,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}
