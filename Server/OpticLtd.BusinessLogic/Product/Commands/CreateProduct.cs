using MediatR;
using OpticLtd.BusinessLogic.Mediator;
using OpticLtd.Data;
using OpticLtd.Data.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Product.Commands
{
  public class CreateProduct
  {
    public class Command : ICommand<Data.Entities.Product>
    {
      public string ProductCategory { get; set; }
      public string ProductName { get; set; }
      public string Description { get; set; }
      public int Stock { get; set; }
      public string Picture { get; set; }
      public string Brand { get; set; }
      public bool? Gender { get; set; }
      public bool? AgeGroup { get; set; }
      public ProductFeature ProductFeature { get; set; } 
    }

    public class Handler : IRequestHandler<Command, Data.Entities.Product>
    {
      private readonly AppDbContext _context;

      public Handler(AppDbContext context)
      {
        _context = context;
      }
      public async Task<Data.Entities.Product> Handle(Command request, CancellationToken cancellationToken)
      {
        var product = new Data.Entities.Product()
        {
          ProductCategory = request.ProductCategory,
          ProductName = request.ProductName,
          Description = request.Description,
          Stock = request.Stock,
          Picture = request.Picture,
          Brand = request.Brand,
          Gender = request.Gender,
          AgeGroup = request.AgeGroup,
          ProductFeature = request.ProductFeature
        };

        _context.Add(product);
        await _context.SaveChangesAsync();

        return product;
      }
    }
  }
}
