using FluentValidation;
using Logiwa.Business.DTOs.Products.Outputs;
using Logiwa.Domain.Entities;
using Logiwa.Infrastructure.DbContexts;
using Logiwa.Infrastructure.Repositories;
using Mapster;
using MediatR;

namespace Logiwa.Business.CQRS.Queries.Products
{
    public class GetFilteredProducts : IRequest<IQueryable<ProductOutputDTO>>
    {
        private readonly GetFilteredProductsValidator _validator = new();
        public GetFilteredProducts(string? searchKey, int? minStock, int? maxStock, int pageSize, int pageNumber)
        {
            SearchKey = searchKey;
            MinStock = minStock;
            MaxStock = maxStock;
            PageSize = pageSize;
            PageNumber = pageNumber;

            _validator.ValidateAndThrow(this);
        }

        public string? SearchKey { get; set; }

        public int? MinStock { get; set; }

        public int? MaxStock { get; set; }

        public int PageSize { get; set; }

        public int PageNumber { get; set; }
    }
    public class GetFilteredProductsValidator : AbstractValidator<GetFilteredProducts>
    {
        public GetFilteredProductsValidator()
        {
            RuleFor(x => x.MinStock)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinStock.HasValue)
                .WithMessage("Minimum stock must be greater than or equal to 0.");
            RuleFor(x => x.MaxStock)
                .GreaterThanOrEqualTo(x => x.MinStock)
                .When(x => x.MaxStock.HasValue && x.MinStock.HasValue)
                .WithMessage("Maximum stock must be greater than or equal to the minimum stock.");
            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");
            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .WithMessage("Page size must be greater than 0.");
        }
    }

    public class GetFilteredProductsHandler : IRequestHandler<GetFilteredProducts, IQueryable<ProductOutputDTO>>
    {
        private readonly IGenericReadRepository<BaseDbContext> _genericReadRepository;

        public GetFilteredProductsHandler(IGenericReadRepository<BaseDbContext> genericReadRepository)
        {
            _genericReadRepository = genericReadRepository;
        }

        public async Task<IQueryable<ProductOutputDTO>> Handle(GetFilteredProducts query, CancellationToken cancellationToken)
        {
            var productQuery = _genericReadRepository.GetAll<Product>();

            if (query.SearchKey != null)
            {
                productQuery = productQuery.Where(x => x.StockCode.Contains(query.SearchKey)
                                                    || x.Description.Contains(query.SearchKey)
                                                    || x.Category.Name.Contains(query.SearchKey));
            }

            if (query.MinStock.HasValue)
            {
                productQuery = productQuery.Where(p => p.StockQuantity >= query.MinStock.Value);
            }

            if (query.MaxStock.HasValue)
            {
                productQuery = productQuery.Where(p => p.StockQuantity <= query.MaxStock.Value);
            }

            productQuery = productQuery.Skip((query.PageNumber - 1) * query.PageSize)
                                       .Take(query.PageSize);

            return await Task.FromResult(productQuery.ProjectToType<ProductOutputDTO>());
        }
    }
}
