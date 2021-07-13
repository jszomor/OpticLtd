using MediatR;

namespace OpticLtd.BusinessLogic.Mediator
{
  public interface ICommand<TResult> : IRequest<TResult> {}
}
