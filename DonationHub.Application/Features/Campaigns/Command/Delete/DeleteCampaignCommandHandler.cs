using DonationHub.Application.Common.Exceptions;
using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Repositories;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Command.Delete;

public class DeleteCampaignCommandHandler : IRequestHandler<DeleteCampaignCommand, Result<Guid>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public DeleteCampaignCommandHandler(
        ICampaignRepository campaignRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _campaignRepository = campaignRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result<Guid>> Handle(DeleteCampaignCommand request, CancellationToken cancellationToken)
    {
        var campaign = await _campaignRepository.GetByIdAsync(request.Id, cancellationToken);
        if (campaign is null)
            return Result<Guid>.Failure(CampaignErrors.NotFound);

        campaign.SoftDelete(_timeProvider.GetUtcNow().UtcDateTime);
        _campaignRepository.Update(campaign);

        try
        {
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            return Result<Guid>.Failure(CampaignErrors.ConcurrencyConflict);
        }

        return Result<Guid>.Success(campaign.Id);
    }
}
