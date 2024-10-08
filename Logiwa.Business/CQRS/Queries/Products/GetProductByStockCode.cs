using FluentValidation;
using Logiwa.Business.DTOs.Products.Outputs;
using Logiwa.Common.Exceptions;
using Logiwa.Domain.Entities;
using Logiwa.Infrastructure.DbContexts;
using Logiwa.Infrastructure.Repositories;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logiwa.Business.CQRS.Queries.Products
{
    public class GetProductByStockCode : IRequest<ProductOutputDTO>
    {
        private readonly GetProductByStockCodeValidator _validator = new();

        public GetProductByStockCode(string stockCode)
        {
            StockCode = stockCode;

            _validator.ValidateAndThrow(this);
        }

        public string StockCode { get; set; }
    }

    public class GetProductByStockCodeValidator : AbstractValidator<GetProductByStockCode>
    {
        public GetProductByStockCodeValidator()
        {
            RuleFor(x => x.StockCode).NotEmpty().NotNull();
        }
    }

    public class GetProductByStockCodeHandler : IRequestHandler<GetProductByStockCode, ProductOutputDTO>
    {
        private readonly IGenericReadRepository<BaseDbContext> _genericReadRepository;

        public GetProductByStockCodeHandler(IGenericReadRepository<BaseDbContext> genericReadRepository)
        {
            _genericReadRepository = genericReadRepository;
        }

        public async Task<ProductOutputDTO> Handle(GetProductByStockCode query, CancellationToken cancellationToken)
        {
            var product = await _genericReadRepository.GetAll<Product>()
                                                       .FirstOrDefaultAsync(x => x.StockCode.Equals(query.StockCode), cancellationToken);

            if (product == null)
            {
                throw new ResourceNotFoundException($"{nameof(Product)} not found with stock code : {query.StockCode}");
            }

            return product.Adapt<ProductOutputDTO>();
        }
    }
}
