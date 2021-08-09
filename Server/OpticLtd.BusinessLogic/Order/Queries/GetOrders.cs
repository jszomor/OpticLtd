using MediatR;
using Microsoft.EntityFrameworkCore;
using OpticLtd.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Order.Queries
{
  public class GetOrders
  {
    public class Query : IRequest<List<Data.Entities.Order>>
    {
      public DateTimeOffset OrderTime { get; set; }
      public string CustomerName { get; set; }
      public string Email { get; set; }
      public Data.Enum.OrderStatus Status { get; set; }
    }

    public class Handler : IRequestHandler<Query, List<Data.Entities.Order>>
    {
      private readonly AppDbContext _context;

      public Handler(AppDbContext context)
      {
        _context = context;
      }

      public async Task<List<Data.Entities.Order>> Handle(Query request, CancellationToken cancellationToken)
      {
        return await _context.Orders
          .Where(o => request.OrderTime.Equals(null) || o.OrderTime.Equals(request.OrderTime))
          .Where(o => request.CustomerName == null || o.CustomerName.StartsWith(request.CustomerName))
          .Where(o => request.Email == null ||o.Email.StartsWith(request.Email))
          .Where(o => request.Status.Equals(null) || o.Status.Equals(request.Status))
          .ToListAsync();
          
          
      }
    }
  }
}
