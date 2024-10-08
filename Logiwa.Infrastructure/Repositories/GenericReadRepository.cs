using Logiwa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logiwa.Infrastructure.Repositories
{

    public class GenericReadRepository<TContext> : IGenericReadRepository<TContext> where TContext : DbContext
    {
        protected readonly TContext _context;

        public GenericReadRepository(TContext context)
        {
            _context = context;
        }
       
        public virtual IQueryable<TEntity> GetAll<TEntity>()
            where TEntity : DomainEntity
        {
            IQueryable<TEntity> dataSet = _context.Set<TEntity>();

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
