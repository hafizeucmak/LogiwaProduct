using FluentValidation;
using Logiwa.Common.Constants;

namespace Logiwa.Domain.Entities
{
    public class Category : DomainEntity
    {
        private readonly CategoryValidator _validator = new();
        private readonly HashSet<Product> _products = new();

        public Category(string name, string description)
        {
            Name = name;
            Description = description;

            _validator.ValidateAndThrow(this);
        }

        public string Name { get; private set; }

        public string Description { get; private set; }

        public int MinimumStockQuantity { get; private set; }

        public bool IsActive { get; private set; }

        public IReadOnlyCollection<Product> Products => _products;

        public class CategoryValidator : AbstractValidator<Category>
        {
            public CategoryValidator()
            {
                RuleFor(c => c.Name).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_CATEGORY_NAME)
                                               .MinimumLength(DbContextConstants.MIN_LENGTH_FOR_CATEGORY_NAME);
                RuleFor(c => c.Description).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_CATEGORY_DESCRIPTIONS);
            }
        }
    }
}
