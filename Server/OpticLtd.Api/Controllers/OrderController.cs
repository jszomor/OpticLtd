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
    public async Task<ActionResult<List<Domain.Model.Order>>> GetOrders([FromQuery] GetOrders.Query query)
    {
      return _mapper.Map<List<Domain.Model.Order>>(await _mediator.Send(query)); 
    }

    [HttpGet]
    [Route("GetOrderById")]
    public async Task<ActionResult<Domain.Model.Order>> GetOrderById([FromBody] GetOrderById.Query query)
    {
      return _mapper.Map<Domain.Model.Order>(await _mediator.Send(query));
    }

    [HttpPost]
    [Route("CreateOrder")]
    public async Task<ActionResult<Domain.Model.Order>> CreateOrder([FromBody] CreateOrder.Command command)
    {
      return _mapper.Map<Domain.Model.Order>(await _mediator.Send(command));
    }

    [HttpDelete]
    [Route("DeleteOrder")]
    public async Task<ActionResult<Domain.Model.Order>> DeleteOrder([FromBody] DeleteOrder.Command command)
    {
      return _mapper.Map<Domain.Model.Order>(await _mediator.Send(command));
    }

    [HttpPut]
    [Route("UpdateOrder")]
    public async Task<ActionResult<Domain.Model.Order>> UpdateOrder([FromBody] UpdateOrder.Command command)
    {
      return _mapper.Map<Domain.Model.Order>(await _mediator.Send(command));
    }
  }
}
