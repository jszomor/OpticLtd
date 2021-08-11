using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpticLtd.Api.Helper;
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
  public class TokenController : ControllerHelper
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly TokenServices _tokenServices;

    public TokenController(UserManager<IdentityUser> userManager, TokenServices tokenServices)
    {
      _userManager = userManager;
      _tokenServices = tokenServices;
    }

    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
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





  }
}
