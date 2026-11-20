using System.Security.Claims;
using DonationHub.Api.Extensions;
using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Campaigns.Command.Create;
using DonationHub.Application.Features.Campaigns.Command.Delete;
using DonationHub.Application.Features.Campaigns.Command.Update;
using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Features.Campaigns.Query.GetAll;
using DonationHub.Application.Features.Campaigns.Query.GetById;
using DonationHub.Application.Features.Campaigns.Query.GetSuccessStories;
using DonationHub.Domain.ResultHandling;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DonationHub.Api.Controllers;

[ApiController]
[Route("api/v1/campaigns")]
public class CampaignsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CampaignsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCampaignsQuery(GetLanguage()), cancellationToken);
        return result.ToActionResult(HttpContext);    }

    [HttpGet("success-stories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSuccessStories(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSuccessStoriesQuery(GetLanguage()), cancellationToken);
        return result.ToActionResult(HttpContext);    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCampaignByIdQuery(id, GetLanguage()), cancellationToken);
        return result.ToActionResult(HttpContext);    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCampaignRequest request, CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
            return Result.Failure(AuthErrors.Unauthorized).ToActionResult(HttpContext);

        var result = await _mediator.Send(new CreateCampaignCommand(request, userId), cancellationToken);
        if (!result.IsSuccess)
            return result.ToActionResult(HttpContext);

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCampaignRequest request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new UpdateCampaignCommand(id, request), cancellationToken);
        return result.ToActionResult(HttpContext);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteCampaignCommand(id), cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToActionResult(HttpContext);
    }

    private string GetLanguage()
    {
        var header = Request.Headers.AcceptLanguage.ToString();
        return header.StartsWith("ar", StringComparison.OrdinalIgnoreCase) ? "ar" : "en";
    }
}
