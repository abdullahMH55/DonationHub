using DonationHub.Domain.Common;

namespace DonationHub.Application.Repositories;

public interface IBaseRepository<T> where T : Entity
{
    Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Delete(T entity);
}