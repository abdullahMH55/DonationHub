using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Categories.DTOs;
using DonationHub.Application.Repositories;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Categories.Command.Update;

public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, Result<UpdateCategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public UpdateCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider; 
    }
    
    public async Task<Result<UpdateCategoryResponse>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Request.Id, cancellationToken);
        if (category == null)
        {
            return Result<UpdateCategoryResponse>.Failure(CategoryErrors.NotFound);
        }

        if (request.Request.NameAr != category.NameAr && await _categoryRepository.ExistsArNameAsync(request.Request.NameAr, cancellationToken))
        {
            return Result<UpdateCategoryResponse>.Failure(CategoryErrors.AlreadyExists);
        }

        if (request.Request.NameEn != category.NameEn && await _categoryRepository.ExistsEnNameAsync(request.Request.NameEn, cancellationToken))
        {
            return Result<UpdateCategoryResponse>.Failure(CategoryErrors.AlreadyExists);
        }

        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        category.UpdateName(request.Request.NameAr, request.Request.NameEn, nowUtc);
        category.UpdateDescription(request.Request.DescriptionAr ?? string.Empty, request.Request.DescriptionEn ?? string.Empty, nowUtc);

        _categoryRepository.Update(category);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<UpdateCategoryResponse>.Success( new UpdateCategoryResponse()
        {
            NameAr = category.NameAr,
            NameEn = category.NameEn,
            DescriptionAr = category.DescriptionAr,
            DescriptionEn = category.DescriptionEn
        });
    }
}
