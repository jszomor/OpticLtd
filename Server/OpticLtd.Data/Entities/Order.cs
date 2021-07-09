using OpticLtd.Data.Enum;
using System;
using System.Collections.Generic;

namespace OpticLtd.Data.Entities
{
  public class Order
  {
    public int OrderId { get; set; }
    public DateTimeOffset OrderTime { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public OrderStatus Status { get; set; }
    public ICollection<OrderItem> OrederItems { get; set; }
  }
}
