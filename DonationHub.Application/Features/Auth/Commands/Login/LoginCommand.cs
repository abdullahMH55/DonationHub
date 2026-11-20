using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Login;

public record LoginCommand(LoginRequest Request) : IRequest<Result<AuthResponce>>;
