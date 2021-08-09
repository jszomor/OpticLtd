using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
  }
}
