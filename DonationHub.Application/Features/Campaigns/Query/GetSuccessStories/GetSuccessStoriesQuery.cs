using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Query.GetSuccessStories;

public record GetSuccessStoriesQuery(string Language) : IRequest<Result<IReadOnlyList<PublicCampaignResponse>>>;
