using DonationHub.Application.Features.Categories.DTOs;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Categories.Query.GetById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<Result<GetAllCategoryResponse>>;