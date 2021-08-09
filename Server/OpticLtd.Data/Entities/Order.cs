using OpticLtd.Data.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpticLtd.Data.Entities
{
  public class Order
  {
    [Key]
    public int OrderId { get; set; }
    public DateTimeOffset OrderTime { get; set; }

    [Required]
    [MaxLength(150)]
    public string CustomerName { get; set; }

    [MaxLength(150)]
    public string PhoneNumber { get; set; }

    [Required]
    [MaxLength(100)]
    [EmailAddress]
    public string Email { get; set; }

    [Column(TypeName = "varchar(50)")]
    public OrderStatus Status { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
  }
}
