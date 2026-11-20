using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Categories.DTOs;
using DonationHub.Application.Repositories;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Categories.Query.GetById;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, Result<GetAllCategoryResponse>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoryByIdHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    

    public async Task<Result<GetAllCategoryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdReadOnlyAsync(request.Id, cancellationToken);
        if (category == null)
        {
            return Result<GetAllCategoryResponse>.Failure(CategoryErrors.NotFound);
        }

        return Result<GetAllCategoryResponse>.Success(new GetAllCategoryResponse
        {
            Id = category.Id,
            NameAr = category.NameAr,
            NameEn = category.NameEn,
            DescriptionAr = category.DescriptionAr,
            DescriptionEn = category.DescriptionEn
        });
    }
}
