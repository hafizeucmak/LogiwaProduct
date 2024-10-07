﻿using Logiwa.Common.Constants;
using Logiwa.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logiwa.Infrastructure.EntityConfigurations
{
    public class ProductEntityConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> product)
        {
            product.HasIndex(x => x.StockCode).IsUnique();

            product.HasIndex(x => new { x.StockCode, x.DeletedAt })
                   .IsUnique()
                   .HasAnnotation(DbContextConstants.CLUSTERED_TEXT, false);

            product.Property(x => x.StockCode)
                   .IsRequired()
                   .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_STOCK_CODE);

            product.Property(x => x.Description)
                   .IsRequired()
                   .HasMaxLength(DbContextConstants.MAX_LENGTH_FOR_PRODUCT_DESCRIPTIONS);

            product.HasOne(x => x.Category)
                   .WithMany()
                   .HasForeignKey(x => x.CategoryId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
