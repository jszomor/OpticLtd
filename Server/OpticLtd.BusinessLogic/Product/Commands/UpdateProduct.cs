using MediatR;
using OpticLtd.BusinessLogic.Mediator;
using OpticLtd.Data;
using OpticLtd.Data.Entities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Product.Commands
{
  public class UpdateProduct
  {
    public class Command : ICommand<Data.Entities.Product>
    {
      public Command()
      {

      }
      public Command(int Id)
      {
        _id = Id;
      }

      public int _id { get; set; }
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
        var product = _context.Products.FirstOrDefault(n => n.ProductId == request._id);

        product.ProductCategory = request.ProductCategory;
        product.ProductName = request.ProductName;
        product.Description = request.Description;
        product.Stock = request.Stock;
        product.Picture = request.Picture;
        product.Brand = request.Brand;
        product.Gender = request.Gender;
        product.AgeGroup = request.AgeGroup;
        product.ProductFeature = request.ProductFeature;

        _context.Update(product);
        await _context.SaveChangesAsync();
        return product;
      }
    }
  }
}
