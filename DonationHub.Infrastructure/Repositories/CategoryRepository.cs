using DonationHub.Application.Repositories;
using DonationHub.Domain.Entities;
using DonationHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DonationHub.Infrastructure.Repositories;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<bool> ExistsArNameAsync(string nameAr, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AnyAsync(c => c.NameAr == nameAr, cancellationToken);
    }

    public async Task<bool> ExistsEnNameAsync(string nameEn, CancellationToken cancellationToken = default)
    {
        return await _context.Categories.AnyAsync(c => c.NameEn == nameEn, cancellationToken);
    }

    public async Task<bool> HasCampaignsAsync(Guid categoryId, CancellationToken cancellationToken = default)
    {
        return await _context.Campaigns.AnyAsync(c => c.CategoryId == categoryId, cancellationToken);
    }
}
