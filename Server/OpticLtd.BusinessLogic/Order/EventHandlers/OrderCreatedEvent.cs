using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Order.EventHandlers
{
  public class OrderCreatedEvent : INotification
  {
    public Data.Entities.Order Order { get; set; }
  }
}
