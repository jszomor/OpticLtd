using MediatR;
using Microsoft.EntityFrameworkCore;
using OpticLtd.Data;
using OpticLtd.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.ShoppingCart.Queries
{
  public class GetShoppingCart
  {
    public class Query : IRequest<List<Data.Entities.CartItem>>
    {
      public int CartItemId { get; set; }
      public string Description { get; set; }
      public int UserId { get; set; }
      public Data.Entities.Product Product { get; set; }
      public int Quantity { get; set; }
    }

    public class Handler : IRequestHandler<Query, List<Data.Entities.CartItem>>
    {
      private readonly AppDbContext _context;

      public Handler(AppDbContext context)
      {
        _context = context;
      }

      public async Task<List<CartItem>> Handle(Query request, CancellationToken cancellationToken)
      {
        var result = await _context.CartItems
          .Where(c => request.CartItemId == 0 || c.CartItemId == request.CartItemId)
          .Where(c => request.Product.ProductId == 0 || c.Product.ProductId == request.Product.ProductId)
          .ToListAsync();

        return result;
      }
    }
  }
}
