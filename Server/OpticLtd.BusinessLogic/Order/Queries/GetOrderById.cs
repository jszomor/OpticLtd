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
  public class GetOrderById
  {
    public class Query : IRequest<Data.Entities.Order>
    {
      public int Id { get; set; }
    }

    public class Handler : IRequestHandler<Query, Data.Entities.Order>
    {
      private readonly AppDbContext _context;

      public Handler(AppDbContext context)
      {
        _context = context;
      }

      public async Task<Data.Entities.Order> Handle(Query request, CancellationToken cancellationToken)
      {
        return await _context.Orders.FindAsync(request.Id);
      }
    }
  }
}
