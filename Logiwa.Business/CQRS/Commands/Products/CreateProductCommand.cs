using FluentValidation;
using Logiwa.Common.Constants;
using Logiwa.Common.Exceptions;
using Logiwa.Domain.Entities;
using Logiwa.Infrastructure.DbContexts;
using Logiwa.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logiwa.Business.CQRS.Commands.Products
{
    public class CreateProductCommand : IRequest<Unit>
    {
        private readonly CreateProductCommandValidator _validator = new();
        public CreateProductCommand(string stockCode, string description, int categoryId, int stockQuantity)
        {
            StockCode = stockCode;
            Description = description;
            CategoryId = categoryId;
            StockQuantity = stockQuantity;

            _validator.ValidateAndThrow(this);
        }

        public string StockCode { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public int StockQuantity { get; set; }
    }

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.StockCode).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_STOCK_CODE);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(DbContextConstants.MAX_LENGTH_FOR_PRODUCT_DESCRIPTIONS);
            RuleFor(x => x.CategoryId).NotEmpty().NotNull().GreaterThan(0).WithMessage($"CategoryId can not be empty.");
            RuleFor(x => x.StockQuantity).NotEmpty().NotNull().WithMessage($"Stock quantity can not be empty.");
        }
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Unit>
    {
        private readonly IGenericWriteRepository<BaseDbContext> _genericWriteRepository;
        private readonly ILogger<CreateProductCommandHandler> _logger;

        public CreateProductCommandHandler(IGenericWriteRepository<BaseDbContext> genericWriteRepository,
                                           ILogger<CreateProductCommandHandler> logger)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            _genericWriteRepository.BeginTransaction();

            var isProductAlreadyExists = await _genericWriteRepository.GetAll<Product>().AnyAsync(x => x.StockCode.Equals(command.StockCode), cancellationToken);

            if (isProductAlreadyExists)
            {
                throw new AlreadyExistsException($"{nameof(Product)} with {nameof(command.StockCode)} is equal to {command.StockCode} already exists.");
            }

            var category = await _genericWriteRepository.GetAll<Category>().FirstOrDefaultAsync(x => x.Id == command.CategoryId, cancellationToken);

            if (category == null)
            {
                throw new ResourceNotFoundException($"{nameof(category)} not found with given Id : {command.CategoryId}");
            }

            var product = new Product(command.StockCode, command.Description, command.StockQuantity, category);

            await _genericWriteRepository.AddAsync(product, cancellationToken);

            return Unit.Value;
        }
    }
}
