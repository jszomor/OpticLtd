using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpticLtd.Api.Configuration;
using OpticLtd.Api.Model.DTOs.Request;
using OpticLtd.Api.Model.DTOs.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpticLtd.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthenticationController : ControllerBase
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly JwtConfig _jwtConfig;

    public AuthenticationController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
    {
      _userManager = userManager;
      _jwtConfig = optionsMonitor.CurrentValue;
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegistration user)
    {
      if (ModelState.IsValid)
      {
        var existingUser = await _userManager.FindByEmailAsync(user.Email);

        if (existingUser != null)
        {
          return BadRequestAuth("Email already in use");
        }

        var newUser = new IdentityUser() { Email = user.Email, UserName = user.UserName };
        var isCreated = await _userManager.CreateAsync(newUser, user.Password);

        if(isCreated.Succeeded)
        {

        }
        else
        {
          return BadRequest(new RegistrationResponse()
          {
            Errors = isCreated.Errors.Select(x => x.Description).ToList(),
            Success = false
          });
        }
      }

      return BadRequestAuth("Invalid payload");      
    }

    private BadRequestObjectResult BadRequestAuth (string message)
    {
      return BadRequest(new RegistrationResponse()
      {
        Errors = new List<string>()
        {
          message
        },
        Success = false
      });
    }
  }
}
