using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Register;

public record RegisterCommand(RegisterRequest Request) : IRequest<Result<AuthResponce>>;
