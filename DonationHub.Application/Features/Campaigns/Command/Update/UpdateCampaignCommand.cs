using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Command.Update;

public record UpdateCampaignCommand(Guid Id, UpdateCampaignRequest Request)
    : IRequest<Result<AdminCampaignResponse>>;
