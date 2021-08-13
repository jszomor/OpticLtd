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
        var result = await _context.Products
          .Where(p => request.Name == null || p.ProductName.StartsWith(request.Name))
          .Where(p => request.Category == null || p.ProductCategory.StartsWith(request.Category))
          .Where(p => request.Gender == null || p.Gender == request.Gender)
          .Where(p => request.AgeGroup == null || p.AgeGroup == request.AgeGroup)
          .Where(p => request.Brand == null || p.Brand.StartsWith(request.Brand))
          .ToListAsync();

        return result;
      }
    }
  }
}
