using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpticLtd.Data.Entities
{
  public class ProductFeatures
  {
    [Key]
    public int FeatureId { get; set; }

    [ForeignKey("Product")]
    public int ProductId { get; set; }

    public Product Product { get; set; }

    [MaxLength(50)]
    public string Color { get; set; }

    [MaxLength(20)]
    public string Gender { get; set; }

    [MaxLength(20)]
    public string AgeGroup { get; set; }

    public double? FrameWidth { get; set; }
    public double? BridgeLength { get; set; }
    public double? StemLength { get; set; }
    public double? LensWidth { get; set; }
    public double? LensHeight { get; set; }

    [MaxLength(50)]
    public string Material { get; set; }

    [MaxLength(50)]
    public string StemType { get; set; }

    public string Accessories { get; set; }
  }
}
