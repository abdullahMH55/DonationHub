using DonationHub.Domain.Entities;

namespace DonationHub.Application.Repositories;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<bool> ExistsArNameAsync(string nameAr, CancellationToken cancellationToken = default);
    Task<bool> ExistsEnNameAsync(string nameEn, CancellationToken cancellationToken = default);
    Task<bool> HasCampaignsAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
