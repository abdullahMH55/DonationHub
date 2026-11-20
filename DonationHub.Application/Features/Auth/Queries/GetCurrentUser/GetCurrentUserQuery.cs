using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Auth.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<Result<CurrentUserResponse>>;
