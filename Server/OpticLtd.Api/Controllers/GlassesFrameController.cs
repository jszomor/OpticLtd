using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpticLtd.Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpticLtd.Api.Controllers
{
  [ApiController]
  public class GlassesFrameController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public GlassesFrameController(IMapper mapper, IMediator mediator)
    {
      _mapper = mapper;
      _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<GlassesFrame>> GetGlassesFrames([FromQuery] )
  }
}
