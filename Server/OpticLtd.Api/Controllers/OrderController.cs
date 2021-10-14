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

  //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager, Admin")]
  [ApiController]
  [Route("api/[controller]/[action]")]
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
    public async Task<ActionResult<List<Domain.Model.Order>>> GetOrder([FromBody] GetOrder.Query query)
    {
      return _mapper.Map<List<Domain.Model.Order>>(await _mediator.Send(query));
    }

    [HttpPost]
    public async Task<ActionResult<Domain.Model.Order>> CreateOrder([FromBody] CreateOrder.Command command)
    {
      return _mapper.Map<Domain.Model.Order>(await _mediator.Send(command));
    }

    [HttpDelete]
    public async Task<ActionResult<Domain.Model.Order>> DeleteOrder([FromBody] DeleteOrder.Command command)
    {
      return _mapper.Map<Domain.Model.Order>(await _mediator.Send(command));
    }

    [HttpPut]
    public async Task<ActionResult<Domain.Model.Order>> UpdateOrder([FromBody] UpdateOrder.Command command)
    {
      return _mapper.Map<Domain.Model.Order>(await _mediator.Send(command));
    }
  }
}
