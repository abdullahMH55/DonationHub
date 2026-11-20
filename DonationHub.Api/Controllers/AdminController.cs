using DonationHub.Api.Extensions;
using DonationHub.Application.Features.Admin.Commands.CreateStaff;
using DonationHub.Application.Features.Auth.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("staff")]
    public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CreateStaffCommand(request), cancellationToken);
        return result.ToActionResult(HttpContext);

    }
}
