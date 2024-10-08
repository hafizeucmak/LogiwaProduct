using FluentValidation;
using Logiwa.Business.DTOs.Products.Outputs;
using Logiwa.Common.Exceptions;
using Logiwa.Domain.Entities;
using Logiwa.Infrastructure.DbContexts;
using Logiwa.Infrastructure.Repositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Logiwa.Business.CQRS.Commands.Products
{
    public class UpdateStockQuantityCommand : IRequest<ProductOutputDTO>
    {
        private readonly UpdateStockQuantityCommandValidator _validator = new();
        public UpdateStockQuantityCommand(string stockCode, int stockQuantity)
        {
            StockCode = stockCode;
            StockQuantity = stockQuantity;

            _validator.ValidateAndThrow(this);
        }

        public string StockCode { get; set; }

        public int StockQuantity { get; set; }
    }

    public class UpdateStockQuantityCommandValidator : AbstractValidator<UpdateStockQuantityCommand>
    {
        public UpdateStockQuantityCommandValidator()
        {
            RuleFor(x => x.StockCode).NotEmpty().NotNull().WithMessage($"Stock code cannot be empty.");
            RuleFor(x => x.StockQuantity).NotEmpty().NotNull().WithMessage($"Stock quantity cannot be empty.");
        }
    }

    public class UpdateStockQuantityCommandHandler : IRequestHandler<UpdateStockQuantityCommand, ProductOutputDTO>
    {
        private readonly IGenericWriteRepository<BaseDbContext> _genericWriteRepository;
        private readonly ILogger<UpdateStockQuantityCommandHandler> _logger;

        public UpdateStockQuantityCommandHandler(IGenericWriteRepository<BaseDbContext> genericWriteRepository,
                                           ILogger<UpdateStockQuantityCommandHandler> logger)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
        }

        public async Task<ProductOutputDTO> Handle(UpdateStockQuantityCommand command, CancellationToken cancellationToken)
        {
            _genericWriteRepository.BeginTransaction();

            var product = await _genericWriteRepository.GetAll<Product>()
                                                       .Include(x => x.Category)
                                                       .FirstOrDefaultAsync(x => x.StockCode.Equals(command.StockCode), cancellationToken);

            if (product == null)
            {
                throw new ResourceNotFoundException($"{nameof(Product)} not found with stock code : {command.StockCode}");
            }

            product.AdjustStockQuantity(command.StockQuantity);
            await _genericWriteRepository.UpdateAsync(product, cancellationToken);

            return product.Adapt<ProductOutputDTO>();
        }
    }
}
