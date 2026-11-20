using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Repositories;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Categories.Command.Delete;

public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, Result<Guid>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<Result<Guid>> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            return Result<Guid>.Failure(CategoryErrors.NotFound);

        var hasCampaigns = await _categoryRepository.HasCampaignsAsync(request.Id, cancellationToken);

        var canDelete = category.EnsureCanBeDeleted(hasCampaigns);
        if (!canDelete.IsSuccess)
            return Result<Guid>.Failure(canDelete.Error!);

        _categoryRepository.Delete(category);
        await _unitOfWork.CompleteAsync(cancellationToken);
        return Result<Guid>.Success(category.Id);
    }
}
