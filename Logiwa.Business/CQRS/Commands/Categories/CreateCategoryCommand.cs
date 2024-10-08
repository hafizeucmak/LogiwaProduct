using FluentValidation;
using Logiwa.Business.DTOs.Categories.Outputs;
using Logiwa.Common.Constants;
using Logiwa.Common.Exceptions;
using Logiwa.Domain.Entities;
using Logiwa.Infrastructure.DbContexts;
using Logiwa.Infrastructure.Repositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logiwa.Business.CQRS.Commands.Categories
{
    public class CreateCategoryCommand : IRequest<CategoryOutputDTO>
    {
        private readonly CreateCategoryCommandValidator _validator = new();
        public CreateCategoryCommand(string name, string description, int minStockQuantity)
        {
            Name = name;
            Description = description;
            MinStockQuantity = minStockQuantity;

            _validator.ValidateAndThrow(this);
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public int MinStockQuantity { get; set; }
    }

    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_CATEGORY_NAME);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_PRODUCT_DESCRIPTIONS);
            RuleFor(x => x.MinStockQuantity).NotEmpty().NotNull().GreaterThan(0).WithMessage($"Minimum Stock quantity can not be empty.");
        }
    }

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryOutputDTO>
    {
        private readonly IGenericWriteRepository<BaseDbContext> _genericWriteRepository;
        private readonly ILogger<CreateCategoryCommandHandler> _logger;

        public CreateCategoryCommandHandler(IGenericWriteRepository<BaseDbContext> genericWriteRepository,
                                           ILogger<CreateCategoryCommandHandler> logger)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
        }

        public async Task<CategoryOutputDTO> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            _genericWriteRepository.BeginTransaction();

            var isCategorylreadyExists = await _genericWriteRepository.GetAll<Category>().AnyAsync(x => x.Name.Equals(command.Name), cancellationToken);

            if (isCategorylreadyExists)
            {
                throw new AlreadyExistsException($"{nameof(Category)} with {nameof(command.Name)} : {command.Name} already exists.");
            }

            var category = new Category(command.Name, command.Description, command.MinStockQuantity);

            await _genericWriteRepository.AddAsync(category, cancellationToken, true);

            return category.Adapt<CategoryOutputDTO>();
        }
    }
}
