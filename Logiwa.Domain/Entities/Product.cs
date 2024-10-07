using FluentValidation;
using Logiwa.Common.Constants;

namespace Logiwa.Domain.Entities
{
    public class Product : DomainEntity
    {
        private readonly ProductValidator _validator = new();

        public Product(string stockCode,
                       string description,
                       int categoryId)
        {
            StockCode = stockCode;
            Description = description;

            _validator.ValidateAndThrow(this);
        }

        public string StockCode { get; private set; }

        public string Description { get; private set; }

        public int CategoryId { get; private set; }

        public virtual Category? Category { get; private set; }

        public class ProductValidator : AbstractValidator<Product>
        {
            public ProductValidator()
            {
                RuleFor(c => c.StockCode).NotEmpty()
                                         .MaximumLength(DbContextConstants.MAX_LENGTH_FOR_STOCK_CODE)
                                         .MinimumLength(DbContextConstants.MIN_LENGTH_FOR_STOCK_CODE);
                RuleFor(c => c.Description).NotEmpty()
                                           .MaximumLength(DbContextConstants.MAX_LENGTH_FOR_PRODUCT_DESCRIPTIONS)
                                           .MinimumLength(DbContextConstants.MIN_LENGTH_FOR_PRODUCT_DESCRIPTIONS);
            }
        }
    }
}
