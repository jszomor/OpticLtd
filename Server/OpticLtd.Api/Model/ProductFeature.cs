using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpticLtd.Api.Model
{
  public class ProductFeature
  {
    public int FeatureId { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }
    public string Color { get; set; }
    public double? FrameWidth { get; set; }
    public double? BridgeLength { get; set; }
    public double? StemLength { get; set; }
    public double? LensWidth { get; set; }
    public double? LensHeight { get; set; }
    public string Material { get; set; }
    public string StemType { get; set; }
    public string Accessories { get; set; }
  }
}
