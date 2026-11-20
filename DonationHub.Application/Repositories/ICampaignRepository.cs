using DonationHub.Domain.Entities;

namespace DonationHub.Application.Repositories;

public interface ICampaignRepository : IBaseRepository<Campaign>
{
    Task<IReadOnlyList<Campaign>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Campaign>> GetCompletedAsync(CancellationToken cancellationToken = default);
    Task<Campaign?> GetByIdWithCategoryAsync(Guid id, CancellationToken cancellationToken = default);
}
