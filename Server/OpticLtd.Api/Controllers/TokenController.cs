using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpticLtd.Api.Helper;
using OpticLtd.BusinessLogic.Services;
using OpticLtd.Data;
using OpticLtd.Domain.DTOs.Request;
using System.Threading.Tasks;

namespace OpticLtd.Api.Controllers
{
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Manager, Admin")]
  [ApiController]
  [Route("api/[controller]")]
  public class TokenController : ControllerHelper
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenServices _tokenServices;

    public TokenController(UserManager<IdentityUser> userManager, ITokenServices tokenServices)
    {
      _userManager = userManager;
      _tokenServices = tokenServices;
    }

    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequestModel tokenRequest)
    {
      if(ModelState.IsValid)
      {
        var result = await _tokenServices.VerifyAndGenerateToken(tokenRequest, _userManager);
        if(result == null)
        {
          BadRequestAuth("Invalid tokens");
        }

        return Ok(result);
      }

      return BadRequestAuth("Invalid payload");
    }
  }
}
