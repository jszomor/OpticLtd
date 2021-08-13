using Microsoft.AspNetCore.Authorization;
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
  [Authorize]
  [ApiController]
  [Route("api/[controller]/[action]")]
  public class AccountController : ControllerHelper
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ITokenServices _tokenServices;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(UserManager<IdentityUser> userManager, ITokenServices tokenServices, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
    {
      _userManager = userManager;
      _tokenServices = tokenServices;
      _signInManager = signInManager;
      _roleManager = roleManager;
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserRegistrationModel user)
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
    public async Task<IActionResult> Logout()
    {
      await _signInManager.SignOutAsync();
      return Ok();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationModel user)
    {

      if (ModelState.IsValid)
      {
        var role = await _roleManager.FindByNameAsync("User");

        var existingUser = await _userManager.FindByEmailAsync(user.Email);

        if (existingUser != null)
        {
          return BadRequestAuth("Email already in use");
        }

        var newUser = new IdentityUser() { Email = user.Email, UserName = user.UserName, PhoneNumber = user.PhoneNumber };
        IdentityResult isCreated = await _userManager.CreateAsync(newUser, user.Password);

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
