using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using OpticLtd.BusinessLogic.Product.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;
using static OpticLtd.BusinessLogic.Product.Queries.GetProducts;

namespace OpticLtd.Api.Controllers
{
  [ApiController]
  [Route ("api/[controller]")]
  public class ProductController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public ProductController(IMapper mapper, IMediator mediator)
    {
      _mapper = mapper;
      _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<Model.Product>>> GetProducts([FromQuery] GetProducts.Query query)
    {
      return _mapper.Map<List<Model.Product>>(await _mediator.Send(query));
    }

  }
}
