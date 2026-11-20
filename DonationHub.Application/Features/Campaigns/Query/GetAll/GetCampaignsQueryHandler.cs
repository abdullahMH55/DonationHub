using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Repositories;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Query.GetAll;

public class GetCampaignsQueryHandler : IRequestHandler<GetCampaignsQuery, Result<IReadOnlyList<PublicCampaignResponse>>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly TimeProvider _timeProvider;

    public GetCampaignsQueryHandler(ICampaignRepository campaignRepository, TimeProvider timeProvider)
    {
        _campaignRepository = campaignRepository;
        _timeProvider = timeProvider;
    }

    public async Task<Result<IReadOnlyList<PublicCampaignResponse>>> Handle(
        GetCampaignsQuery request,
        CancellationToken cancellationToken)
    {
        var campaigns = await _campaignRepository.GetActiveAsync(cancellationToken);
        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;

        var response = campaigns
            .Select(campaign => campaign.ToPublicResponse(request.Language, nowUtc))
            .ToList();

        return Result<IReadOnlyList<PublicCampaignResponse>>.Success(response);
    }
}
