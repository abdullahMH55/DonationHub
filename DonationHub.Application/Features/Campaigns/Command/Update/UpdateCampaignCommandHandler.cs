using DonationHub.Application.Common.Exceptions;
using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Repositories;
using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Command.Update;

public class UpdateCampaignCommandHandler : IRequestHandler<UpdateCampaignCommand, Result<AdminCampaignResponse>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public UpdateCampaignCommandHandler(
        ICampaignRepository campaignRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider)
    {
        _campaignRepository = campaignRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _timeProvider = timeProvider;
    }

    public async Task<Result<AdminCampaignResponse>> Handle(
        UpdateCampaignCommand request,
        CancellationToken cancellationToken)
    {
        var campaign = await _campaignRepository.GetByIdAsync(request.Id, cancellationToken);
        if (campaign is null)
            return Result<AdminCampaignResponse>.Failure(CampaignErrors.NotFound);

        if (!campaign.RowVersion.SequenceEqual(request.Request.RowVersion))
            return Result<AdminCampaignResponse>.Failure(CampaignErrors.ConcurrencyConflict);

        var category = await _categoryRepository.GetByIdAsync(request.Request.CategoryId, cancellationToken);
        if (category is null)
            return Result<AdminCampaignResponse>.Failure(CategoryErrors.NotFound);

        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        var updateResult = campaign.Update(
            request.Request.TitleAr,
            request.Request.TitleEn,
            request.Request.DescriptionAr,
            request.Request.DescriptionEn,
            request.Request.GoalAmount,
            request.Request.EndDate,
            request.Request.CategoryId,
            nowUtc);

        if (!updateResult.IsSuccess)
            return Result<AdminCampaignResponse>.Failure(updateResult.Error!);

        _campaignRepository.Update(campaign);

        try
        {
            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (ConcurrencyConflictException)
        {
            return Result<AdminCampaignResponse>.Failure(CampaignErrors.ConcurrencyConflict);
        }

        return Result<AdminCampaignResponse>.Success(campaign.ToAdminResponse());
    }
}
