
using OpticLtd.Data.Entities;

namespace OpticLtd.Api.Model
{
  public class Product
  {
    public int ProductId { get; set; }
    public string ProductCategory { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public int Stock { get; set; }
    public string Picture { get; set; }
    public string Brand { get; set; }
    public bool Gender { get; set; }
    public bool AgeGroup { get; set; }
    public Data.Entities.ProductFeature ProductFeature { get; set; }
  }
}
