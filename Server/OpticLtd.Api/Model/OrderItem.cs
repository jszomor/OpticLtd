namespace OpticLtd.Api.Model
{
  public class OrderItem
  {
    public int OrderItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public Product Product { get; set; }
    public Order Order { get; set; }
  }
}
