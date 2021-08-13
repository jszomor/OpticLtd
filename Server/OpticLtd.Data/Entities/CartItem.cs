using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpticLtd.Data.Entities
{
  public class CartItem
  {
    [Key]
    public int CartItemId { get; set; }
    public string Description { get; set; }
    public string UserId { get; set; }

    [ForeignKey("ProductId")]
    public Product Product { get; set; }
    public int Quantity { get; set; }
  }
}
