using Logiwa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logiwa.Infrastructure.Repositories
{
    public class GenericWriteRepository<TContext> : IGenericWriteRepository<TContext> where TContext : DbContext
    {
        protected readonly TContext _context;

        public GenericWriteRepository(TContext context)
        {
            _context = context;
        }

        public virtual IQueryable<TEntity> GetAll<TEntity>()
             where TEntity : DomainEntity
        {
            return _context.Set<TEntity>();
        }

        public virtual async Task<TEntity> GetByIdAsync<TEntity>(int id, CancellationToken cancellationToken)
            where TEntity : DomainEntity
        {
            TEntity? entity = await GetAll<TEntity>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (entity == null)
            {
                throw new ArgumentNullException($"Entity of type {typeof(TEntity).Name} with ID {id} not found.");
            }
            return entity;
        }

        public virtual IQueryable<TEntity> GetAllAsNoTracking<TEntity>()
            where TEntity : DomainEntity
        {
            return GetAll<TEntity>().AsNoTracking();
        }

        public virtual async Task<TEntity> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken, bool saveChanges = false)
                where TEntity : DomainEntity
        {
            await _context.Set<TEntity>().AddAsync(entity, cancellationToken);

            if (saveChanges)
            {
               await _context.SaveChangesAsync(cancellationToken);
            }

            return entity;
        }

        public virtual void Detach<TEntity>(TEntity entity)
            where TEntity : DomainEntity
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        public virtual void DetachAll<TEntity>(IEnumerable<TEntity> entities)
            where TEntity : DomainEntity
        {
            foreach (var entity in entities)
            {
                Detach(entity);
            }
        }

        public virtual async Task<int> RemoveAsync<TEntity>(TEntity entity, CancellationToken cancellationToken, bool saveChanges = false)
            where TEntity : DomainEntity
        {
            int resultCount = 0;
            _context.Entry(entity).State = EntityState.Deleted;
            if (saveChanges)
            {
                resultCount = await _context.SaveChangesAsync(cancellationToken);
            }

            return resultCount;
        }

        public virtual async Task<int> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken, bool saveChanges = false)
            where TEntity : DomainEntity
        {
            int resultCount = 0;
            entity.Update();
            _context.Entry(entity).State = EntityState.Modified;
            _context.ChangeTracker.DetectChanges();

            if (saveChanges)
            {
                resultCount = await _context.SaveChangesAsync(cancellationToken);
            }

            return resultCount;
        }

        public async Task<TEntity> GetByIdThrowsAsync<TEntity>(int id, CancellationToken cancellationToken)
            where TEntity : DomainEntity
        {
            var result = await GetByIdAsync<TEntity>(id, cancellationToken);

            return result;
        }

        public void BeginTransaction()
        {
            _context.Database.BeginTransaction();
        }

        public void CommitTransaction()
        {
            if (_context.Database.CurrentTransaction != null)
            {
                _context.SaveChanges();
                _context.Database.CurrentTransaction.Commit();
            }
        }

        public void RollbackTransaction()
        {
            _context.Database.CurrentTransaction?.Rollback();
        }
    }
}
