using FluentValidation;
using Logiwa.Common.Constants;

namespace Logiwa.Domain.Entities
{
    public class Product : DomainEntity
    {
        private readonly ProductValidator _validator = new();

        public Product(string stockCode,
                       string description,
                       int stockQuantity,
                       Category category)
        {
            Category = category ?? throw new ArgumentNullException(nameof(category));

            StockCode = stockCode;
            Description = description;
            StockQuantity = stockQuantity;
            CategoryId = category.Id;

            UpdateIsActive();

            _validator.ValidateAndThrow(this);
        }

        protected Product() { }

        public string StockCode { get; private set; }

        public string Description { get; private set; }

        public int CategoryId { get; private set; }

        public virtual Category Category { get; private set; }

        public int StockQuantity { get; private set; }

        public bool IsActive {  get; private set; }

        public void UpdateCategory(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            Category = category;
            CategoryId = category.Id;
            UpdateIsActive();
            Update();
        }

        private void UpdateIsActive()
        {
            if (this.Category == null)
            {
                throw new ArgumentNullException($"{nameof(Category)} cannot be empty to update product active status.");
            }

            IsActive = StockQuantity >= Category?.MinimumStockQuantity && Category != null;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
            Update();
        }

        public void AdjustStockQuantity(int quantity)
        {
            StockQuantity += quantity;

            if (StockQuantity < 0)
            {
                throw new InvalidOperationException("Stock quantity cannot be negative");
            }

            UpdateIsActive();
        }

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
                RuleFor(c => c.Category).NotEmpty().NotNull();
                RuleFor(c => c.StockQuantity).NotEmpty().NotNull();
            }
        }
    }
}
