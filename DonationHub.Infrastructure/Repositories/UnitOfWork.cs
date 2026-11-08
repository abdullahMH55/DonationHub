using DonationHub.Application.Repositories;
using DonationHub.Infrastructure.Data;

namespace DonationHub.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    AppDbContext _dbContext;

    public UnitOfWork(AppDbContext dbContext)
    {
        _dbContext  = dbContext;
    }
    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }
}