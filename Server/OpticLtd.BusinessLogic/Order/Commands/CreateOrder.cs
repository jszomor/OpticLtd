using MediatR;
using OpticLtd.BusinessLogic.Mediator;
using OpticLtd.Data;
using OpticLtd.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Order.Commands
{
  public class CreateOrder
  {
    public class Command : ICommand<Data.Entities.Order>
    {
      public string CustomerName { get; set; }
      public string PhoneNumber { get; set; }
      public string Email { get; set; }
      public ICollection<OrderItem> OrderItems { get; set; }
      public Data.Entities.Product Product { get; set; }
    }

    public class Handler : IRequestHandler<Command, Data.Entities.Order>
    {
      private readonly AppDbContext _context;

      public Handler(AppDbContext context)
      {
        _context = context;
      }

      public async Task<Data.Entities.Order> Handle(Command request, CancellationToken cancellationToken)
      {
        var order = new Data.Entities.Order()
        {
          CustomerName = request.CustomerName,
          Email = request.Email,
          PhoneNumber = request.PhoneNumber,
          OrderTime = DateTime.UtcNow,
          Status = Data.Enum.OrderStatus.Active,
          OrderItems = new List<OrderItem>()
          {
            new OrderItem()
            {
              Product = request.Product
            }            
          }          
        };

        await _context.AddAsync(order);
        await _context.SaveChangesAsync();
        return order;
      }
    }
  }
}
