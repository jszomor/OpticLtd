
using Newtonsoft.Json;

namespace OpticLtd.Domain.Model
{
  public class Product
  {
    [JsonProperty("productId")]
    public int ProductId { get; set; }

    [JsonProperty("productCategory")]
    public string ProductCategory { get; set; }

    [JsonProperty("productName")]
    public string ProductName { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("stock")]
    public int Stock { get; set; }

    [JsonProperty("picture")]
    public string Picture { get; set; }

    [JsonProperty("brand")]
    public string Brand { get; set; }

    [JsonProperty("gender")]
    public bool? Gender { get; set; }

    [JsonProperty("ageGroup")]
    public bool? AgeGroup { get; set; }

    [JsonProperty("featureId")]
    public int FeatureId { get; set; }

    [JsonProperty("productFeature")]
    public ProductFeatureApi ProductFeature { get; set; }
  }
}
