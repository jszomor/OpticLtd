﻿using MediatR;
using OpticLtd.BusinessLogic.Mediator;
using OpticLtd.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Order.Commands
{
  public class DeleteOrder
  {
    public class Command : ICommand<Data.Entities.Order>
    {
      public int Id { get; set; }
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
        var order = _context.Orders.FirstOrDefault(x => x.OrderId == request.Id);
        var orderItem = _context.OrderItem.FirstOrDefault(x => x.OrderItemId == request.Id);
        _context.Remove(order);
        _context.Remove(orderItem);
        await _context.SaveChangesAsync();
        return order;
      }
    }
  }
}
