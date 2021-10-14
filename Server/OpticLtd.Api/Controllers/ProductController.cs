using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpticLtd.BusinessLogic.Product.Commands;
using OpticLtd.BusinessLogic.Product.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpticLtd.Api.Controllers
{
  //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager, Admin")]
  [ApiController]
  [Route("api/[controller]")]
  public class ProductController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IMapper mapper, IMediator mediator, ILogger<ProductController> logger)
    {
      _mapper = mapper;
      _mediator = mediator;
      _logger = logger;
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("GetProducts")]
    public async Task<ActionResult<List<Domain.Model.Product>>> GetProducts([FromQuery] GetProducts.Query query)
    {
      try
      {
        var getProducts = _mapper.Map<List<Domain.Model.Product>>(await _mediator.Send(query));
        _logger.LogInformation("GetProducts response ok.");
        return getProducts;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Bad response from GetProducts.");
        return null;
      }
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("GetProductById")]
    public async Task<ActionResult<Domain.Model.Product>> GetProductById([FromBody] GetProductById.Query query)
    {
      return _mapper.Map<Domain.Model.Product>(await _mediator.Send(query));
    }

    [HttpPost]
    [Route("CreateProduct")]
    public async Task<ActionResult<Domain.Model.Product>> CreateProduct([FromBody] CreateProduct.Command command)
    {
      return _mapper.Map<Domain.Model.Product>(await _mediator.Send(command));
    }

    [HttpDelete]
    [Route("DeleteProduct")]
    public async Task<ActionResult<Domain.Model.Product>> DeleteProduct([FromBody] DeleteProduct.Command command)
    {
      return _mapper.Map<Domain.Model.Product>(await _mediator.Send(command));
    }

    [HttpPut]
    [Route("UpdateProduct")]
    public async Task<ActionResult<Domain.Model.Product>> UpdateProduct([FromBody] UpdateProduct.Command command)
    {
      return _mapper.Map<Domain.Model.Product>(await _mediator.Send(command));
    }
  }
}
