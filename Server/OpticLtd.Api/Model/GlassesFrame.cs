using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace OpticLtd.Api.Model
{
  public class GlassesFrame
  {
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public int Stock { get; set; }
    public string Brand { get; set; }
    public string Picture { get; set; }
    public string Color { get; set; }
    public string Gender { get; set; }
    public string AgeGroup { get; set; }
    public string ProductType { get; set; }
    public double FrameWidth { get; set; }
    public double Bridge { get; set; }
    public double StemLength { get; set; }
    public double LensWidth { get; set; }
    public double LensHeight { get; set; }
    public string Material { get; set; }
    public string StemType { get; set; }
    public string Accessories { get; set; }
  }
}
