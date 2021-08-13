using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpticLtd.BusinessLogic.ShoppingCart.Commands;
using OpticLtd.BusinessLogic.ShoppingCart.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpticLtd.Api.Controllers
{
  [Authorize]
  [Route("api/[controller]/[action]")]
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin, Manager, Customer")]
  public class ShoppingCartController : ControllerBase
  {
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public ShoppingCartController(IMapper mapper, IMediator mediator, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
      _mapper = mapper;
      _mediator = mediator;
      _userManager = userManager;
      _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<ActionResult<List<Domain.Model.CartItem>>> GetShoppingCart(GetShoppingCart.Query query)
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var userName = User.FindFirstValue(ClaimTypes.Name);

      if (User.Identity.IsAuthenticated)
      {
        return _mapper.Map<List<Domain.Model.CartItem>>(await _mediator.Send(query));
      }
      else
      {
        return BadRequest("No user logged in!");
      }
    }

    [HttpPost]
    public async Task<ActionResult<List<Domain.Model.CartItem>>> AddProductToCart(AddProductToCart.Command request)
    {
      var currentUserId = ((ClaimsIdentity)User.Identity).FindFirst("Id").Value;

      request.UserId = currentUserId;



      return Ok();
    }

  }
}
