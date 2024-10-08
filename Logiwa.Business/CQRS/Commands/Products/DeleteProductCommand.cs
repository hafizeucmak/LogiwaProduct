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
    public class DeleteProductCommand : IRequest<Unit>
    {
        private readonly DeleteProductCommandValidator _validator = new();
        public DeleteProductCommand(string stockCode)
        {
            StockCode = stockCode;

            _validator.ValidateAndThrow(this);
        }

        public string StockCode { get; set; }
    }

    public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
    {
        public DeleteProductCommandValidator()
        {
            RuleFor(x => x.StockCode).NotEmpty().NotNull().WithMessage($"Stock code cannot be empty.");
        }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
    {
        private readonly IGenericWriteRepository<BaseDbContext> _genericWriteRepository;
        private readonly ILogger<DeleteProductCommandHandler> _logger;

        public DeleteProductCommandHandler(IGenericWriteRepository<BaseDbContext> genericWriteRepository,
                                           ILogger<DeleteProductCommandHandler> logger)
        {
            _genericWriteRepository = genericWriteRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            _genericWriteRepository.BeginTransaction();

            var product = await _genericWriteRepository.GetAll<Product>()
                                                       .FirstOrDefaultAsync(x => x.StockCode.Equals(command.StockCode), cancellationToken);

            if (product == null)
            {
                throw new ResourceNotFoundException($"{nameof(Product)} not found with stock code : {command.StockCode}");
            }

            product.Delete();
            await _genericWriteRepository.UpdateAsync(product, cancellationToken);

            return Unit.Value;
        }
    }
}
