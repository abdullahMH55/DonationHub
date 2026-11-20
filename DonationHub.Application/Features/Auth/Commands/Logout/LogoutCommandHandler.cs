using DonationHub.Application.Services;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IAuthService _authService;

    public LogoutCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        return _authService.RevokeTokenAsync(request.RefreshToken, cancellationToken);
    }
}
