using DonationHub.Application.Repositories;
using DonationHub.Domain.Entities;
using DonationHub.Domain.Enums;
using DonationHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Repositories;

public class CampaignRepository : BaseRepository<Campaign>, ICampaignRepository
{
    public CampaignRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Campaign>> GetActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Campaigns
            .AsNoTracking()
            .Include(c => c.Category)
            .Where(c => c.Status == CampaignStatus.Active)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Campaign>> GetCompletedAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Campaigns
            .AsNoTracking()
            .Include(c => c.Category)
            .Where(c => c.Status == CampaignStatus.Completed)
            .ToListAsync(cancellationToken);
    }

    public async Task<Campaign?> GetByIdWithCategoryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Campaigns
            .AsNoTracking()
            .Include(c => c.Category)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
