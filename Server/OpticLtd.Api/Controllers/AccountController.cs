using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpticLtd.Api.Helper;
using OpticLtd.BusinessLogic.Services;
using OpticLtd.Domain.DTOs.Request;
using OpticLtd.Domain.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpticLtd.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AccountController : ControllerHelper
  {
    private readonly UserManager<IdentityUser> _userManager;
    public TokenServices _tokenServices;

    public AccountController(UserManager<IdentityUser> userManager, TokenServices tokenServices)
    {
      _userManager = userManager;
      _tokenServices = tokenServices;
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserRegistration user)
    {
      if (ModelState.IsValid)
      {
        var existingUser = await _userManager.FindByEmailAsync(user.Email);

        if (existingUser == null)
        {
          BadRequestAuth("Invalid login request");
        }

        var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

        if (!isCorrect)
        {
          BadRequestAuth("Invalid login request");
        }

        var jwtToken = await _tokenServices.GenerateJwtToken(existingUser);

        return Ok(jwtToken);
      }
      return BadRequestAuth("Invalid payload");
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

        var newUser = new IdentityUser() { Email = user.Email, UserName = user.UserName, PhoneNumber = user.PhoneNumber };
        var isCreated = await _userManager.CreateAsync(newUser, user.Password);

        if (isCreated.Succeeded)
        {
          var jwtToken = await _tokenServices.GenerateJwtToken(newUser);

          return Ok(jwtToken);
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
  }
}
