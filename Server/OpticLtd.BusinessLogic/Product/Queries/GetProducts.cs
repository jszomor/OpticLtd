using MediatR;
using Microsoft.EntityFrameworkCore;
using OpticLtd.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Product.Queries
{
  public class GetProducts
  {
    public class Query : IRequest<List<Data.Entities.Product>>
    {
      public string Name { get; set; }
      public string Category { get; set; }
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
        return await _context.Products
          .Where(p => p.ProductName == null || p.ProductName.StartsWith(request.Name))
          .Where(p => p.ProductCategory == null || p.ProductCategory.StartsWith(request.Category))
          .Where(p => p.Gender == null || p.Gender == request.Gender)
          .Where(p => p.AgeGroup == null || p.AgeGroup == request.AgeGroup)
          .Where(p => p.Brand == null || p.Brand.StartsWith(request.Brand))
          .ToListAsync();
      }
    }
  }
}
