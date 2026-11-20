using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Repositories;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Query.GetSuccessStories;

public class GetSuccessStoriesQueryHandler : IRequestHandler<GetSuccessStoriesQuery, Result<IReadOnlyList<PublicCampaignResponse>>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly TimeProvider _timeProvider;

    public GetSuccessStoriesQueryHandler(ICampaignRepository campaignRepository, TimeProvider timeProvider)
    {
        _campaignRepository = campaignRepository;
        _timeProvider = timeProvider;
    }

    public async Task<Result<IReadOnlyList<PublicCampaignResponse>>> Handle(
        GetSuccessStoriesQuery request,
        CancellationToken cancellationToken)
    {
        var campaigns = await _campaignRepository.GetCompletedAsync(cancellationToken);
        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;

        var response = campaigns
            .Select(campaign => campaign.ToPublicResponse(request.Language, nowUtc))
            .ToList();

        return Result<IReadOnlyList<PublicCampaignResponse>>.Success(response);
    }
}
