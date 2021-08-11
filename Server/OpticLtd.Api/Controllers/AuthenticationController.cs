using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpticLtd.BusinessLogic.Services;
using OpticLtd.Data;
using OpticLtd.Domain.DTOs.Request;
using OpticLtd.Domain.DTOs.Response;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OpticLtd.Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthenticationController : ControllerBase
  {
    private readonly UserManager<IdentityUser> _userManager;
  
    private readonly RoleManager<IdentityRole> _roleManager;

    AuthServices AuthServices = new();

    public AuthenticationController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor, AppDbContext context, RoleManager<IdentityRole> roleManager)
    {
      _userManager = userManager;
      _context = context;
      _jwtConfig = optionsMonitor.CurrentValue;
      _roleManager = roleManager;
    }



    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
      if(ModelState.IsValid)
      {
        var result = await AuthServices.VerifyAndGenerateToken(tokenRequest, _userManager);
        if(result == null)
        {
          BadRequestAuth("Invalid tokens");
        }

        return Ok(result);
      }

      return BadRequestAuth("Invalid payload");
    }






    //[HttpPost]
    //[Route("CreateRole")]
    //public async Task<IActionResult> CreateRole(RoleViewModel vm)
    //{
    //  await _roleManager.CreateAsync(new IdentityRole { Name = vm.Name });

    //  return RedirectToAction("Index");
    //}

    //[HttpPost]
    //[Route("UpdateRole")]
    //public async Task<IActionResult> UpdateUserRole(UpdateUserRoleViewModel vm)
    //{
    //  var user = await _userManager.FindByEmailAsync(vm.UserEmail);

    //  if (vm.Delete)
    //    await _userManager.RemoveFromRoleAsync(user, vm.Role);
    //  else
    //    await _userManager.AddToRoleAsync(user, vm.Role);

    //  return RedirectToAction("Index");
    //}

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

        if(isCreated.Succeeded)
        {
          var jwtToken = await GenerateJwtToken(newUser);

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

    private BadRequestObjectResult BadRequestAuth(string message)
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
