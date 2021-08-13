using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpticLtd.BusinessLogic.Order.Commands;
using OpticLtd.BusinessLogic.Order.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpticLtd.Api.Controllers
{

  [ApiController]
  [Route("api/[controller]")]
  //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager, Admin")]
  public class OrderController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public OrderController(IMapper mapper, IMediator mediator)
    {
      _mapper = mapper;
      _mediator = mediator;
    }

    [HttpGet]
    [Route("GetOrders")]
    public async Task<ActionResult<List<Domain.Model.Order>>> GetOrders(GetOrders.Query query)
    {
      return _mapper.Map<List<Domain.Model.Order>>(await _mediator.Send(query));
    }

    [HttpGet("{id:int}")]
    [Route("GetOrderById")]
    public async Task<ActionResult<Domain.Model.Order>> GetOrderById([FromQuery] GetOrderById.Query query)
    {
      return _mapper.Map<Domain.Model.Order>(await _mediator.Send(query.Id));
    }

    [HttpPost]
    [Route("CreateOrder")]
    public async Task<ActionResult> CreateOrder([FromBody] CreateOrder.Command request)
    {
      Data.Entities.Order order = await _mediator.Send(request);
      return CreatedAtAction(nameof(GetOrders), new { orderId = order.OrderId }, _mapper.Map<Domain.Model.Order>(order));
    }

    [HttpDelete("{id:int}")]
    [Route("DeleteOrder")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
      _mapper.Map<Domain.Model.Order>(await _mediator.Send(new DeleteOrder.Command(id)));

      return NoContent();
    }

    [HttpPut]
    [Route("UpdateOrder")]
    public async Task<ActionResult<Domain.Model.Order>> UpdateOrder([FromQuery] UpdateOrder.Command command)
    {
      return _mapper.Map<Domain.Model.Order>(await _mediator.Send(command));
    }
  }
}
