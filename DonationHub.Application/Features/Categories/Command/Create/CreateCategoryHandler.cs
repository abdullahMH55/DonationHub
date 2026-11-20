using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Categories.DTOs;
using DonationHub.Application.Repositories;
using DonationHub.Domain.ResultHandling;
using MediatR;
using Category = DonationHub.Domain.Entities.Category;

namespace DonationHub.Application.Features.Categories.Command.Create;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public CreateCategoryHandler(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider)
    {
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider; 
    }
    
    public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var existsArName = await _categoryRepository.ExistsArNameAsync(request.Request.NameAr, cancellationToken);
        var existsEnName = await _categoryRepository.ExistsEnNameAsync(request.Request.NameEn, cancellationToken);

        if (existsArName || existsEnName)
            return Result<CreateCategoryResponse>.Failure(CategoryErrors.AlreadyExists);

        var category = Category.Create(
            request.Request.NameAr,
            request.Request.NameEn,
            request.Request.DescriptionAr ?? string.Empty,
            request.Request.DescriptionEn ?? string.Empty,
            _timeProvider.GetUtcNow().UtcDateTime
        );

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<CreateCategoryResponse>.Success(new CreateCategoryResponse()
        {
            Id = category.Id,
            NameAr = category.NameAr,
            NameEn = category.NameEn,
            DescriptionAr = category.DescriptionAr,
            DescriptionEn = category.DescriptionEn
        });
    }
}
