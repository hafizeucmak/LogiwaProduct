using Logiwa.Domain.Entities;
using Mapster;

namespace Logiwa.Business.DTOs.Products.Outputs
{
    public class ProductOutputDTO
    {
        public int Id { get; set; }
        public string StockCode { get; set; }
        public string Description { get; set; }
        public int StockQuantity { get; set; }
        public string CategoryName { get; set; }
        public bool IsActive { get; set; }
    }

    public class ProductOutputDTOCustomMap : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<Product, ProductOutputDTO>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.StockCode, src => src.StockCode)
                .Map(dest => dest.StockQuantity, src => src.StockQuantity)
                .Map(dest => dest.IsActive, src => src.IsActive)
                .Map(dest => dest.CategoryName, src => src.Category.Name);
        }
    }
}
