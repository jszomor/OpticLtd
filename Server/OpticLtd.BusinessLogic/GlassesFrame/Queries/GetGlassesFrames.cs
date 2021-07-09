using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.GlassesFrame.Queries
{
  public class GetGlassesFrames
  {
    public class Query : IRequest<List<Data.Entities.GlassesFrame>>
    {
      public string Name { get; set; }
      public decimal UnitPrice { get; set; }
      public string Gender { get; set; }
      public string AgeGroup { get; set; }
    }

    public class Handler : IRequestHandler<Query, List<Data.Entities.GlassesFrame>>
    {

      public Handler()
      {

      }

      public Task<List<Data.Entities.GlassesFrame>> Handle(Query request, CancellationToken cancellationToken)
      {
        throw new NotImplementedException();
      }
    }
  }
}
