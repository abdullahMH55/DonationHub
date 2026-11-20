using DonationHub.Application.ErrorCatalogue;
using DonationHub.Application.Features.Campaigns.DTOs;
using DonationHub.Application.Repositories;
using DonationHub.Domain.ResultHandling;
using MediatR;
using CampaignEntity = DonationHub.Domain.Entities.Campaign;

namespace DonationHub.Application.Features.Campaigns.Command.Create;

public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, Result<AdminCampaignResponse>>
{
    private readonly ICampaignRepository _campaignRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeProvider _timeProvider;

    public CreateCampaignCommandHandler(
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
        CreateCampaignCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Request.CategoryId, cancellationToken);
        if (category is null)
            return Result<AdminCampaignResponse>.Failure(CategoryErrors.NotFound);

        var nowUtc = _timeProvider.GetUtcNow().UtcDateTime;
        var createResult = CampaignEntity.Create(
            request.Request.TitleAr,
            request.Request.TitleEn,
            request.Request.DescriptionAr,
            request.Request.DescriptionEn,
            request.Request.GoalAmount,
            request.Request.EndDate,
            request.Request.CategoryId,
            request.CreatedByUserId,
            nowUtc,
            request.Request.BannerFileName);

        if (!createResult.IsSuccess)
            return Result<AdminCampaignResponse>.Failure(createResult.Error);

        var campaign = createResult.Value;
        await _campaignRepository.AddAsync(campaign, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<AdminCampaignResponse>.Success(campaign.ToAdminResponse());
    }
}
