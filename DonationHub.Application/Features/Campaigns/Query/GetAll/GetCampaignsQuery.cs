using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Query.GetAll;

public record GetCampaignsQuery(string Language) : IRequest<Result<IReadOnlyList<PublicCampaignResponse>>>;
