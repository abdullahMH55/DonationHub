using DonationHub.Application.Repositories;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<bool> ExistsArNameAsync(string nameAr, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories.AnyAsync(c => c.NameAr == nameAr, cancellationToken);
    }

    public async Task<bool> ExistsEnNameAsync(string nameEn,  CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories.AnyAsync(c => c.NameEn == nameEn, cancellationToken);
    }

    public async Task<bool> HasCampaignsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Campaigns
            .IgnoreQueryFilters()
            .AnyAsync(c => c.CategoryId == categoryId, cancellationToken);
    }

    public async Task<Category?> GetByIdReadOnlyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
