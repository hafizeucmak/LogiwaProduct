using Logiwa.Domain.Entities;
using Logiwa.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Logiwa.Infrastructure.Repositories
{
    public class GenericReadRepository : IGenericReadRepository
    {
        protected readonly BaseDbContext _baseDb;

        public Guid ContextScopeId { get; }

        public GenericReadRepository(BaseDbContext baseDb)
        {
            _baseDb = baseDb;
        }

        public virtual IQueryable<TEntity> GetAll<TEntity>()
            where TEntity : DomainEntity
        {
            IQueryable<TEntity> dataSet = _baseDb.Set<TEntity>();

            return dataSet.AsNoTracking();
        }

        public async Task<TEntity> GetByIdThrowsAsync<TEntity>(int id, CancellationToken cancellationToken)
            where TEntity : DomainEntity
        {
            var result = await GetAll<TEntity>().FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);
            if (result == null)
            {
                string entityName = typeof(TEntity).Name;
                throw new ArgumentNullException($"{entityName} with selected Id : {id} not found.");
            }

            return result;
        }
    }
}
