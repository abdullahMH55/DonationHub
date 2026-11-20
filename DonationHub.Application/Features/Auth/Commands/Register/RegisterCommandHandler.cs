using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Application.Services;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponce>>
{
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<Result<AuthResponce>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return _authService.RegisterAsync(request.Request, cancellationToken);
    }
}
