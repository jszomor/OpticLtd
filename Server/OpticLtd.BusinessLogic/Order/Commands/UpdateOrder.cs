using MediatR;
using OpticLtd.BusinessLogic.Mediator;
using OpticLtd.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Order.Commands
{
  public class UpdateOrder
  {
    public class Command : ICommand<Data.Entities.Order>
    {
      public int Id { get; set; }
      public string CustomerName { get; set; }
      public string PhoneNumber { get; set; }
      public string Email { get; set; }
      public ICollection<Data.Entities.OrderItem> OrderItems { get; set; }
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
        Data.Entities.Order order = _context.Orders.FirstOrDefault(o => o.OrderId == request.Id);
        if(order == null)
        {
          return null;
        }

        order.CustomerName = request.CustomerName ?? order.CustomerName;
        order.PhoneNumber = request.PhoneNumber ?? order.PhoneNumber;
        order.Email = request.Email ?? order.Email;
        order.OrderItems = request.OrderItems ?? order.OrderItems;

        _context.Update(order);
        await _context.SaveChangesAsync();
        return order;
      }
    }
  }
}
