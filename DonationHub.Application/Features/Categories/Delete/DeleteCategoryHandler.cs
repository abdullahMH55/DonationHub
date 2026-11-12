using DonationHub.Application.Repositories;

namespace DonationHub.Application.Features.Categories.Delete;

public class DeleteCategoryHandler
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> HandleAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryRepository.GetByIdAsync(id, cancellationToken);
        if (category == null)
        {
            return false;
        }

        var hasCampaigns = await _categoryRepository.HasCampaignsAsync(id, cancellationToken);
        if (hasCampaigns)
        {
            return false;
        }

        _categoryRepository.Delete(category);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return true;
    }
}
