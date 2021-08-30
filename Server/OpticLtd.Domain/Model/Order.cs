using OpticLtd.Data.Enum;
using System;
using System.Collections.Generic;

namespace OpticLtd.Domain.Model
{
  public class Order
  {
    public int OrderId { get; set; }
    public DateTimeOffset OrderTime { get; set; }
    public string CustomerName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public OrderStatus Status { get; set; }
    public ICollection<Model.OrderItem> OrederItems { get; set; }
  }
}
