using Logiwa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logiwa.Infrastructure.Repositories
{
    public interface IGenericReadRepository<TContext> where TContext : DbContext
    {
        IQueryable<TEntity> GetAll<TEntity>()
            where TEntity : DomainEntity;

        Task<TEntity> GetByIdThrowsAsync<TEntity>(int id, CancellationToken cancellationToken)
            where TEntity : DomainEntity;
    }
}
