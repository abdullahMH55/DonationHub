using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<AuthResponce>>;
