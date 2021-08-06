using MediatR;
using OpticLtd.BusinessLogic.Order.EventHandlers;
using OpticLtd.BusinessLogic.Product.Queries;
using OpticLtd.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Product.EventHandlers
{
  public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
  {
    private readonly AppDbContext _context;

    public OrderCreatedEventHandler(AppDbContext context)
    {
      _context = context;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
      foreach (var item in notification.Order.OrderItems)
      {
        await ChangeProductStockAsync(item.ProductId, -item.Amount);
      }
    }

    private async Task<int> ChangeProductStockAsync(int productId, int changeStock)
    {
      var getProduct = new GetProductById.Handler(_context);
      var product = await getProduct.FindProductByIdAsync(productId);

      int newStock = product.Stock + changeStock;

      if(product.Stock <= 0)
      {
        throw new InvalidOperationException("Not enough stock!");
      }

      product.Stock = newStock;
      await _context.SaveChangesAsync();

      return product.Stock;
    }
  }
}
