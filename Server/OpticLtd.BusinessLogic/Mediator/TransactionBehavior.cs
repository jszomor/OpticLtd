using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using OpticLtd.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Mediator
{
  public class TransactionBehavior<TRequest, TResult> : IPipelineBehavior<TRequest, TResult>
  {
    private readonly AppDbContext _context;

    public TransactionBehavior(AppDbContext context)
    {
      _context = context;
    }

    public async Task<TResult> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResult> next)
    {
      using var transaction = await _context.Database.BeginTransactionAsync();

      try
      {
        var result = await next();
        await transaction.CommitAsync();
        return result;
      }
      catch (Exception)
      {
        await transaction.RollbackAsync();
        throw;
      }
    }
  }
}
