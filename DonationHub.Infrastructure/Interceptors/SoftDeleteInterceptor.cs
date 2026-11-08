using System.Data.Common;
using DonationHub.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DonationHub.Infrastructure.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{


    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if(eventData.Context == null)
            return ValueTask.FromResult(result);
        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Deleted, Entity: ISoftDeletable entity })
                continue;
            entry.State = EntityState.Modified;
            entity.IsDeleted = true;
            entity.DeletedAtUtc =  DateTime.UtcNow;
        }

        return ValueTask.FromResult(result);  
    }
}