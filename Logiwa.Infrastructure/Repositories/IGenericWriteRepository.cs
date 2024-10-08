﻿using Logiwa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logiwa.Infrastructure.Repositories
{
    public interface IGenericWriteRepository<TContext> where TContext : DbContext
    {
        IQueryable<TEntity> GetAll<TEntity>()
            where TEntity : DomainEntity;

        IQueryable<TEntity> GetAllAsNoTracking<TEntity>()
            where TEntity : DomainEntity;

        Task<TEntity> GetByIdAsync<TEntity>(int id, CancellationToken cancellationToken)
            where TEntity : DomainEntity;

        Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken, bool saveChanges = false)
            where TEntity : DomainEntity;

        void Detach<TEntity>(TEntity entity)
            where TEntity : DomainEntity;

        void DetachAll<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : DomainEntity;

        Task<int> RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken, bool saveChanges = false)
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
