using DonationHub.Application.Features.Categories.DTOs;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Categories.Command.Update;

public record UpdateCategoryCommand(UpdateCategoryRequest Request) : IRequest<Result<UpdateCategoryResponse>>;