﻿using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpticLtd.BusinessLogic.Product.Commands;
using OpticLtd.BusinessLogic.Product.Queries;
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
        _logger.LogInformation("GetProducts response ok.");
        return _mapper.Map<List<Domain.Model.Product>>(await _mediator.Send(query));
      }
      catch (System.Exception)
      {
        _logger.LogError("Bad response from GetProducts.");
        return null;
      }
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Domain.Model.Product>> GetProductById(int id)
    {
      return _mapper.Map<Domain.Model.Product>(await _mediator.Send(new GetProductById.Query(id)));
    }

    [HttpPost]
    public async Task<ActionResult> CreateProduct([FromBody] CreateProduct.Command request)
    {
      Data.Entities.Product product = await _mediator.Send(request);
      return CreatedAtAction(nameof(GetProducts), new { productId = product.ProductId }, _mapper.Map<Domain.Model.Product>(product));
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
      _mapper.Map<Domain.Model.Product>(await _mediator.Send(new DeleteProduct.Command(id)));

      return NoContent();
    }

    [HttpPut]
    public async Task<ActionResult<Domain.Model.Product>> UpdateProduct([FromQuery] UpdateProduct.Command command)
    {
      return _mapper.Map<Domain.Model.Product>(await _mediator.Send(command));
    }
  }
}
