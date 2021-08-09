﻿using AutoMapper;
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
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "Admin")]

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
    public async Task<ActionResult<List<Api.Model.Order>>> GetOrders([FromQuery] GetOrders.Query query)
    {
      return _mapper.Map<List<Api.Model.Order>>(await _mediator.Send(query));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Model.Order>> GetProductById([FromQuery] GetOrderById.Query query)
    {
      return _mapper.Map<Model.Order>(await _mediator.Send(query.Id));
    }

    [HttpPost]
    public async Task<ActionResult> CreateOrder([FromBody] CreateOrder.Command request)
    {
      Data.Entities.Order order = await _mediator.Send(request);
      return CreatedAtAction(nameof(GetOrders), new { orderId = order.OrderId }, _mapper.Map<Model.Order>(order));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteOrder(int id)
    {
      _mapper.Map<Model.Order>(await _mediator.Send(new DeleteOrder.Command(id)));

      return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult<Model.Order>> UpdateProduct([FromQuery] UpdateOrder.Command command)
    {
      return _mapper.Map<Model.Order>(await _mediator.Send(command));
    }
  }
}
