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
      public int ProductId { get; set; }
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
        var product = _context.Products.FirstOrDefault(n => n.ProductId == request.ProductId);

        if (product == null)
          return null;

        product.ProductCategory = request.ProductCategory != null ? request.ProductCategory : product.ProductCategory;
        product.ProductName = request.ProductName != null ? request.ProductName : product.ProductName;
        product.Description = request.Description != null ? request.Description : product.Description;
        product.Stock = request.Stock;
        product.Picture = request.Picture != null ? request.Picture : product.Picture;
        product.Brand = request.Brand != null ? request.Brand : product.Brand;
        product.Gender = request.Gender != null ? request.Gender : product.Gender;
        product.AgeGroup = request.AgeGroup != null ? request.AgeGroup : product.AgeGroup;
        product.ProductFeature = request.ProductFeature != null ? request.ProductFeature : product.ProductFeature;

        _context.Update(product);
        await _context.SaveChangesAsync();
        return product;
      }
    }
  }
}
