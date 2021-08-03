using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpticLtd.Data.Entities
{
  public class Product
  {
    [Key]
    public int ProductId { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProductCategory { get; set; }

    [Required]
    [MaxLength(200)]
    public string ProductName { get; set; }
    public string Description { get; set; }
    public int Stock { get; set; }

    [MaxLength(200)]
    public string Picture { get; set; }

    [MaxLength(50)]
    public string Brand { get; set; }

    public bool? Gender { get; set; }

    public bool? AgeGroup { get; set; }

    [ForeignKey ("FeatureId")]
    public ProductFeature ProductFeature { get; set; }

  }
}
