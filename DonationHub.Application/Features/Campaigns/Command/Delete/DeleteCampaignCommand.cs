using DonationHub.Domain.ResultHandling;
using MediatR;

namespace DonationHub.Application.Features.Campaigns.Command.Delete;

public record DeleteCampaignCommand(Guid Id) : IRequest<Result<Guid>>;
