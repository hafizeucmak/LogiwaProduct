using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Logiwa.WebAPI.Filters
{
    [ExcludeFromCodeCoverage]
    public class TransactionManagerFilter<TDbContext> : IActionFilter where TDbContext : DbContext
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            var dbContext = GetDbContext(context);

            if (context.Exception != null)
            {
                RollbackTransaction(dbContext);
            }
            else
            {
                CommitTransaction(dbContext);
            }
        }

        private TDbContext GetDbContext(ActionExecutedContext context)
        {
            var dbContext =  (TDbContext?)context.HttpContext.RequestServices.GetService(typeof(TDbContext));

            if (dbContext == null)
            {
                throw new InvalidOperationException($"The requested service of type {typeof(TDbContext).Name} is not registered.");
            }

            return dbContext;
        }

        private void RollbackTransaction(TDbContext dbContext)
        {
            if (dbContext.Database.CurrentTransaction != null)
            {
                dbContext.Database.RollbackTransaction();
            }
        }

        private void CommitTransaction(TDbContext dbContext)
        {
            dbContext.SaveChanges();

            if (dbContext.Database.CurrentTransaction != null)
            {
                dbContext.Database.CurrentTransaction.Commit();
            }
        }
    }
}
