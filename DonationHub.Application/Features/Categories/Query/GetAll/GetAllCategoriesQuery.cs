using DonationHub.Application.Features.Categories.DTOs;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Categories.Query.GetAll;

public record GetAllCategoriesQuery() : IRequest<Result<List<GetAllCategoryResponse>>>;