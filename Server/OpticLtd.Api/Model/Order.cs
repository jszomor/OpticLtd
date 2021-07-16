using OpticLtd.Data.Enum;
using System;
using System.Collections.Generic;

namespace OpticLtd.Api.Model
{
  public class Order
  {
    public int OrderId { get; set; }
    public DateTimeOffset OrderTime { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public OrderStatus Status { get; set; }
    public ICollection<Model.OrderItem> OrederItems { get; set; }
  }
}
