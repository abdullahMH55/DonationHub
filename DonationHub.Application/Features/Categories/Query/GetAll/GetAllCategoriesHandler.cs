using DonationHub.Application.Features.Categories.DTOs;
using DonationHub.Application.Repositories;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Categories.Query.GetAll;

public class GetAllCategoriesHandler : IRequestHandler<GetAllCategoriesQuery, Result<List<GetAllCategoryResponse>>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetAllCategoriesHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }
    

    public async Task<Result<List<GetAllCategoryResponse>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return Result<List<GetAllCategoryResponse>>.Success(categories.Select(c => new GetAllCategoryResponse
        {
            Id = c.Id,
            NameAr = c.NameAr,
            NameEn = c.NameEn,
            DescriptionAr = c.DescriptionAr,
            DescriptionEn = c.DescriptionEn
        }).ToList());    }
}
