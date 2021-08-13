using MediatR;
using OpticLtd.BusinessLogic.Mediator;
using OpticLtd.Data.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.ShoppingCart.Commands
{
  public class AddProductToCart
  {
    public class Command : ICommand<Data.Entities.CartItem>
    {
      public string Description { get; set; }
      public string UserId { get; set; }
      public Data.Entities.Product Product { get; set; }
      public int Quantity { get; set; }
    }

    public class Handler : IRequestHandler<Command, Data.Entities.CartItem>
    {
      public Task<CartItem> Handle(Command request, CancellationToken cancellationToken)
      {
        throw new NotImplementedException();
      }
    }
  }
}
