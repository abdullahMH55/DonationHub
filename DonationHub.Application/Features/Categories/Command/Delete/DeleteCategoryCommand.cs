using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Categories.Command.Delete;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result<Guid>>;