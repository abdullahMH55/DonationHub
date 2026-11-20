using DonationHub.Application.Common.Exceptions;
using DonationHub.Application.Repositories;
using DonationHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

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
        try
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new ConcurrencyConflictException("A concurrency conflict occurred.", exception);
        }
    }
}