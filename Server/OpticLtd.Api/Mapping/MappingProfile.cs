using AutoMapper;

namespace OpticLtd.Api.Mapping
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      CreateMap<Data.Entities.Product, Model.Product>();
      CreateMap<Data.Entities.ProductFeature, Model.ProductFeature>();
      CreateMap<Data.Entities.Order, Model.Order>();
      CreateMap<Data.Entities.OrderItem, Model.OrderItem>();
    }
  }
}
