using DonationHub.Application.Services;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Admin.Commands.CreateStaff;

public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, Result<Guid>>
{
    private readonly IAuthService _authService;

    public CreateStaffCommandHandler(IAuthService authService)
    {
        _authService = authService;
    }

    public Task<Result<Guid>> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        return _authService.CreateStaffAsync(request.Request, cancellationToken);
    }
}
