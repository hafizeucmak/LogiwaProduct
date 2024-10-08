using Logiwa.Domain.Entities;
using Mapster;

namespace Logiwa.Business.DTOs.Categories.Outputs
{
    public class CategoryOutputDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int MinimumStockCount { get; set; }
    }

    public class CategoryOutputDTOCustomMap : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<Category, CategoryOutputDTO>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.MinimumStockCount, src => src.MinimumStockQuantity)
                .Map(dest => dest.Description, src => src.Description);
        }
    }
}
