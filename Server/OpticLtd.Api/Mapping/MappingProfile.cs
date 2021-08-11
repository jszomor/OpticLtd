using AutoMapper;

namespace OpticLtd.Api.Mapping
{
  public class MappingProfile : Profile
  {
    public MappingProfile()
    {
      CreateMap<Data.Entities.Product, Domain.Model.Product>();
      CreateMap<Data.Entities.ProductFeature, Domain.Model.ProductFeatureApi>();
      CreateMap<Data.Entities.Order, Domain.Model.Order>();
      CreateMap<Data.Entities.OrderItem, Domain.Model.OrderItem>();
      CreateMap<Data.Entities.RefreshToken, Domain.Model.RefreshToken>();
    }
  }
}
