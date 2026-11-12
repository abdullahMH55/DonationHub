using DonationHub.Application.Features.Categories.DTOs;
using DonationHub.Application.Repositories;

namespace DonationHub.Application.Features.Categories.Update;

public class UpdateCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CategoryResponse?> HandleAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return null;
        }

        if (request.NameAr != category.NameAr && await _categoryRepository.ExistsArNameAsync(request.NameAr, cancellationToken))
        {
            return null;
        }

        if (request.NameEn != category.NameEn && await _categoryRepository.ExistsEnNameAsync(request.NameEn, cancellationToken))
        {
            return null;
        }

        category.UpdateName(request.NameAr, request.NameEn, DateTime.UtcNow);
        category.UpdateDescription(request.DescriptionAr ?? string.Empty, request.DescriptionEn ?? string.Empty, DateTime.UtcNow);

        _categoryRepository.Update(category);
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
