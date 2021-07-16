using MediatR;
using OpticLtd.BusinessLogic.Mediator;
using OpticLtd.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Product.Commands
{
  public class DeleteProduct
  {
    public class Command : ICommand<Data.Entities.Product>
    {
      public int _id { get; set; }

      public Command(int Id)
      {
        _id = Id;
      }
      public Command()
      {

      }
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
        var product = _context.Products.FirstOrDefault(x => x.ProductId == request._id);
        _context.Remove(product);
        await _context.SaveChangesAsync();
        return product;
      }
    }
  }
}
