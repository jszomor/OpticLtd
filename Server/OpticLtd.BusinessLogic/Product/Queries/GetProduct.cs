using MediatR;
using Microsoft.EntityFrameworkCore;
using OpticLtd.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Product.Queries
{
  public class GetProduct
  {
    public class Query : IRequest<List<Data.Entities.Product>>
    {
      public int? ProductId { get; set; }
      public string ProductName { get; set; }
      public string ProductCategory { get; set; }
      public bool? Gender { get; set; }
      public bool? AgeGroup { get; set; }
      public string Brand { get; set; }
    }

    public class Handler : IRequestHandler<Query, List<Data.Entities.Product>>
    {
      private readonly AppDbContext _context;

      public Handler(AppDbContext context)
      {
        _context = context;
      }

      public async Task<List<Data.Entities.Product>> Handle(Query request, CancellationToken cancellationToken)
      {
        var result = await _context.Products
          .Where(p => request.ProductId == null || p.ProductId == request.ProductId)
          .Where(p => request.ProductName == null || p.ProductName.StartsWith(request.ProductName))
          .Where(p => request.ProductCategory == null || p.ProductCategory.StartsWith(request.ProductCategory))
          .Where(p => request.Gender == null || p.Gender == request.Gender)
          .Where(p => request.AgeGroup == null || p.AgeGroup == request.AgeGroup)
          .Where(p => request.Brand == null || p.Brand.StartsWith(request.Brand))
          .ToListAsync();

        return result;
      }

      public async Task<Data.Entities.Product> FindProductByIdAsync(int id)
      {
        return await _context.Products.FindAsync(id);
      }
    }
  }
}
