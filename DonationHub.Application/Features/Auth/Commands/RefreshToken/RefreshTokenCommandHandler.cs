using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Application.Services;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponce>>
{
    private readonly IAuthService _authService;

    public RefreshTokenCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<Result<AuthResponce>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        return _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
    }
}
