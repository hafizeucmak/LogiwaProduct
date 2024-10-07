using Logiwa.Domain.Entities;

namespace Logiwa.Infrastructure.Repositories
{
    public interface IGenericReadRepository
    {
        IQueryable<TEntity> GetAll<TEntity>()
            where TEntity : DomainEntity;

        Task<TEntity> GetByIdThrowsAsync<TEntity>(int id, CancellationToken cancellationToken)
            where TEntity : DomainEntity;
    }
}
