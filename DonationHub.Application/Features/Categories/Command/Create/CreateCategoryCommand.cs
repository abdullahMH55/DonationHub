using DonationHub.Application.Features.Categories.DTOs;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Categories.Command.Create;

public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<Result<CreateCategoryResponse>>;
