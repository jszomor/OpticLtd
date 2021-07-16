using MediatR;
using OpticLtd.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Product.Queries
{
  public class GetProductById
  {
    public class Query : IRequest<Data.Entities.Product>
    {
      public int _id { get; set; }
      public Query(int Id)
      {
        _id = Id;
      }
      public Query()
      {

      }
    }
    public class Handler : IRequestHandler<Query, Data.Entities.Product>
    {
      private readonly AppDbContext _context;

      public Handler(AppDbContext context)
      {
        _context = context;
      }

      public async Task<Data.Entities.Product> Handle(Query request, CancellationToken cancellationToken)
      {
        return await FindProductByIdAsync(request._id);
      }

      public async Task<Data.Entities.Product> FindProductByIdAsync(int id)
      {
        return await _context.Products.FindAsync(id);
      }
    }
  }
}
