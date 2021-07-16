using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpticLtd.Data.Entities
{
  public class ProductFeature
  {
    [Key]
    public int FeatureId { get; set; }

    [MaxLength(50)]
    public string Color { get; set; }

    public double? FrameWidth { get; set; }
    public double? BridgeLength { get; set; }
    public double? StemLength { get; set; }
    public double? LensWidth { get; set; }
    public double? LensHeight { get; set; }

    [MaxLength(50)]
    public string Material { get; set; }

    [MaxLength(50)]
    public string StemType { get; set; }

    [MaxLength(300)]
    public string Accessories { get; set; }
  }
}
