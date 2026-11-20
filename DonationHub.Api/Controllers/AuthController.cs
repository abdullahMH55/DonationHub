using System.Security.Claims;
using DonationHub.Api.Extensions;
using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Auth.Commands.Login;
using DonationHub.Application.Features.Auth.Commands.Logout;
using DonationHub.Application.Features.Auth.Commands.RefreshToken;
using DonationHub.Application.Features.Auth.Commands.Register;
using DonationHub.Application.Features.Auth.DTO;
using DonationHub.Application.Features.Auth.Queries.GetCurrentUser;
using DonationHub.Domain.ResultHandling;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RegisterCommand(request), cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LoginCommand(request), cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken), cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new LogoutCommand(request.RefreshToken), cancellationToken);
        return result.ToActionResult(HttpContext);    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
            return Result.Failure(AuthErrors.Unauthorized).ToActionResult(HttpContext);

        var result = await _mediator.Send(new GetCurrentUserQuery(userId), cancellationToken);
        return result.ToActionResult(HttpContext);
    }
}
