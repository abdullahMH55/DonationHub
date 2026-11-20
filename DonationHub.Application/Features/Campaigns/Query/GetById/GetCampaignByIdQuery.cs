using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Query.GetById;

public record GetCampaignByIdQuery(Guid Id, string Language) : IRequest<Result<PublicCampaignResponse>>;
