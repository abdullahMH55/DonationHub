using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Repositories;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Query.GetById;

public class GetCampaignByIdQueryHandler : IRequestHandler<GetCampaignByIdQuery, Result<PublicCampaignResponse>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly TimeProvider _timeProvider;

    public GetCampaignByIdQueryHandler(ICampaignRepository campaignRepository, TimeProvider timeProvider)
    {
        _campaignRepository = campaignRepository;
        _timeProvider = timeProvider;
    }

    public async Task<Result<PublicCampaignResponse>> Handle(
        GetCampaignByIdQuery request,
        CancellationToken cancellationToken)
    {
        var campaign = await _campaignRepository.GetByIdWithCategoryAsync(request.Id, cancellationToken);
        if (campaign is null)
            return Result<PublicCampaignResponse>.Failure(CampaignErrors.NotFound);

        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        return Result<PublicCampaignResponse>.Success(campaign.ToPublicResponse(request.Language, nowUtc));
    }
}
