using Logiwa.Common.Constants;
using Logiwa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace Logiwa.Infrastructure.DbContexts
{
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
            ChangeTracker.LazyLoadingEnabled = false;
            ChangeTracker.AutoDetectChangesEnabled = false;

        }
        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            var entityTypes = GetEntityTypes(modelBuilder).ToList();

            AddDefaultMaxLength(modelBuilder, entityTypes);
            AddGlobalQueryFilters(modelBuilder, entityTypes);

            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }

        private IEnumerable<IMutableEntityType> GetEntityTypes(ModelBuilder builder)
        {
            return builder.Model.GetEntityTypes().Where(x => typeof(IDomainEntity).IsAssignableFrom(x.ClrType));
        }

        private void AddDefaultMaxLength(ModelBuilder builder, IEnumerable<IMutableEntityType> entityTypes)
        {
            foreach (var entityType in entityTypes)
            {
                var entityClrType = entityType.ClrType;
                var entityTypeBuilder = builder.Entity(entityClrType);
                var properties = entityClrType.GetProperties();

                var stringProperties = properties.Where(x => x.PropertyType == typeof(string));

                foreach (var stringPropertyInfo in stringProperties)
                {
                    entityTypeBuilder
                        .Property(stringPropertyInfo.PropertyType, stringPropertyInfo.Name)
                        .HasMaxLength(DbContextConstants.DEFAULT_MAX_LENGTH_FOR_STRING);
                }

                var enumProperties = properties.Where(x => x.PropertyType.IsEnum);

                foreach (var enumPropertyInfo in enumProperties)
                {
                    entityTypeBuilder
                        .Property(enumPropertyInfo.PropertyType, enumPropertyInfo.Name)
                        .HasMaxLength(DbContextConstants.DEFAULT_MAX_LENGTH_FOR_STRING);
                }
            }
        }

        private void AddGlobalQueryFilters(ModelBuilder modelBuilder, IEnumerable<IMutableEntityType> entityTypes)
        {
            foreach (var entityType in entityTypes)
            {
                if (typeof(DomainEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");

                    var deletedAtProperty = Expression.Call(
                        typeof(EF),
                        nameof(EF.Property),
                        new[] { typeof(DateTime?) },
                        parameter,
                        Expression.Constant(nameof(DomainEntity.DeletedAt))
                    );

                    var compareToNull = Expression.Equal(deletedAtProperty, Expression.Constant(null, typeof(DateTime?)));

                    var lambda = Expression.Lambda(compareToNull, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
