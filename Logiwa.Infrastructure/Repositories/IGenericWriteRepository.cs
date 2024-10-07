using Logiwa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Logiwa.Infrastructure.Repositories
{
    public interface IGenericWriteRepository<TContext> where TContext : DbContext
    {
        IQueryable<TEntity> GetAll<TEntity>(
            [CallerFilePath] string callerPath = "",
            [CallerMemberName] string callerMemberName = "")
            where TEntity : DomainEntity;

        IQueryable<TEntity> GetAllAsNoTracking<TEntity>()
            where TEntity : DomainEntity;

        Task<TEntity> GetByIdAsync<TEntity>(int id, CancellationToken cancellationToken)
            where TEntity : DomainEntity;

        Task<int> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken, bool saveChanges = false)
            where TEntity : DomainEntity;

        Task<int> AddRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveChanges = false)
            where TEntity : DomainEntity;

        void Detach<TEntity>(TEntity entity)
            where TEntity : DomainEntity;

        void DetachAll<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : DomainEntity;

        Task<int> RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken, bool saveChanges = false)
            where TEntity : DomainEntity;

        Task<int> RemoveRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveChanges = false)
            where TEntity : DomainEntity;

        Task<int> UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool saveChanges = false)
            where TEntity : DomainEntity;

        Task<int> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken, bool saveChanges = false)
            where TEntity : DomainEntity;

        Task<TEntity> GetByIdThrowsAsync<TEntity>(int id, CancellationToken cancellationToken)
            where TEntity : DomainEntity;

        void BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}
