using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpticLtd.BusinessLogic.Product.Commands;
using OpticLtd.BusinessLogic.Product.Queries;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpticLtd.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]  
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
    public async Task<ActionResult<List<Api.Model.Product>>> GetProducts([FromQuery] GetProducts.Query query)
    {
      return _mapper.Map<List<Api.Model.Product>>(await _mediator.Send(query));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Model.Product>> GetProductById(int id)
    {
      return _mapper.Map<Model.Product>(await _mediator.Send(new GetProductById.Query(id)));
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult> CreateProduct([FromBody] CreateProduct.Command request)
    {
      Data.Entities.Product product = await _mediator.Send(request);
      return CreatedAtAction(nameof(GetProducts), new { productId = product.ProductId }, _mapper.Map<Model.Product>(product));
    }

    [HttpDelete("{id:int}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
      _mapper.Map<Model.Product>(await _mediator.Send(new DeleteProduct.Command(id)));

      return NoContent();
    }

    [HttpPut]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public async Task<ActionResult<Model.Product>> UpdateProduct([FromQuery] UpdateProduct.Command command)
    {
      return _mapper.Map<Model.Product>(await _mediator.Send(command));
    }
  }
}
