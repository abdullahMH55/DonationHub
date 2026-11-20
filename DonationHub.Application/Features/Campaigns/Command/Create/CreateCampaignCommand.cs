using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Command.Create;

public record CreateCampaignCommand(CreateCampaignRequest Request, Guid CreatedByUserId) : IRequest<Result<AdminCampaignResponse>>;
