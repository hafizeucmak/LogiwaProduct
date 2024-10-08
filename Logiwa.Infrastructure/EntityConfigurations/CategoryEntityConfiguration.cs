using Logiwa.Common.Constants;
using Logiwa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logiwa.Infrastructure.EntityConfigurations
{
    public class CategoryEntityConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> category)
        {
            category.HasIndex(x => x.Name).IsUnique();

            category.HasIndex(x => new { x.Name, x.DeletedAt })
                    .IsUnique()
                    .HasAnnotation(DbContextConstants.CLUSTERED_TEXT, false);

            category.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_CATEGORY_NAME);

            category.HasIndex(x => new { x.Name, x.DeletedAt })
                   .IsUnique()
                   .HasAnnotation(DbContextConstants.CLUSTERED_TEXT, false);

            category.Property(x => x.Description)
                   .IsRequired()
                   .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_CATEGORY_DESCRIPTIONS);

            category.HasMany(c => c.Products)
                    .WithOne(p => p.Category)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
