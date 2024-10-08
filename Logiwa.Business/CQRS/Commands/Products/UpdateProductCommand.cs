using FluentValidation;
using Logiwa.Common.Exceptions;
using Logiwa.Domain.Entities;
using Logiwa.Infrastructure.DbContexts;
using Logiwa.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logiwa.Business.CQRS.Commands.Products
{
    public class UpdateProductCommand : IRequest<Unit>
    {
        private readonly UpdateProductCommandValidator _validator = new();
        public UpdateProductCommand(string stockCode, string? description, int? categoryId)
        {
            StockCode = stockCode;
            Description = description;
            CategoryId = categoryId;

            _validator.ValidateAndThrow(this);
        }

        public string StockCode { get; set; }

        public string? Description { get; set; }

        public int? CategoryId { get; set; }
    }

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.StockCode).NotEmpty().NotNull().WithMessage($"Stock code cannot be empty.");
        }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
    {
        private readonly IGenericWriteRepository<BaseDbContext> _genericWriteRepository;
        private readonly ILogger<UpdateProductCommandHandler> _logger;

        public UpdateProductCommandHandler(IGenericWriteRepository<BaseDbContext> genericWriteRepository,
                                           ILogger<UpdateProductCommandHandler> logger)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            _genericWriteRepository.BeginTransaction();

            var product = await _genericWriteRepository.GetAll<Product>()
                                                       .FirstOrDefaultAsync(x => x.StockCode.Equals(command.StockCode), cancellationToken);

            if (product == null)
            {
                throw new ResourceNotFoundException($"{nameof(Product)} not found with stock code : {command.StockCode}");
            }

            if (command.CategoryId != null && command.CategoryId != product.CategoryId)
            {
                var category = _genericWriteRepository.GetAll<Category>()
                                                       .FirstOrDefault(x => x.Id == command.CategoryId);

                if (category == null)
                {
                    throw new ResourceNotFoundException($"{nameof(category)} not found with given Id : {command.CategoryId}");
                }

                product.UpdateCategory(category);
            }

            if (command.Description != null)
            {
                product.UpdateDescription(command.Description);
            }

            await _genericWriteRepository.UpdateAsync(product, cancellationToken);

            return Unit.Value;
        }
    }
}
