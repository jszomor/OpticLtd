using Microsoft.AspNetCore.Authentication.JwtBearer;
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
  [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
  [ApiController]
  [Route("api/[controller]")]
  public class AdminController : ControllerHelper
  {

    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenServices _tokenServices;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ITokenServices tokenServices)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _tokenServices = tokenServices;
    }

    #region User Management

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] UserRegistrationModel user)
    {
      if (ModelState.IsValid)
      {
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

    [HttpPost]
    public async Task<IActionResult> EditUser(IdentityUser user)
    {
      if (!ModelState.IsValid) return BadRequestAuth("Invalid payload");

      var existingUser = await _userManager.FindByIdAsync(user.Id);

      if (existingUser != null)
      {
        existingUser.Email = user.Email ?? existingUser.Email;
        existingUser.UserName = user.UserName ?? existingUser.UserName;
        existingUser.PhoneNumber = user.PhoneNumber ?? existingUser.PhoneNumber;

        var result = await _userManager.UpdateAsync(existingUser);
        if (result.Succeeded)
        {
          return Ok();
        }
        else
        {
          return BadRequestAuth("Update operation failed");
        }
      }
      else
      {
        return BadRequestAuth("Operation failed. Selected user does not exist.");
      }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(string userId)
    {
      IdentityUser user = await _userManager.FindByIdAsync(userId);

      if (user != null)
      {
        IdentityResult result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
          return Ok();
        }
        else
        {
          return BadRequestAuth("Delete operation failed");
        }
      }
      else
      {
        return BadRequestAuth("Operation failed. Selected user does not exist.");
      }
    }

    #endregion

    #region Role Management

    [HttpPost]
    [Route("CreateRole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
      string[] validRoleNames = new[] { "Admin", "Manager", "User" };

      if (validRoleNames.Contains(roleName))
      {
        if (await _roleManager.RoleExistsAsync(roleName) == false)
        {
          await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
        else
        {
          return BadRequestAuth("Given role is already exist.");
        }
      }
      else
      {
        return BadRequestAuth("Invalid role name.");
      }

      return Ok();
    }

    [HttpPost]
    [Route("UpdateRole")]
    public async Task<IActionResult> UpdateUserRole(UpdateUserRoleViewModel vm)
    {
      var user = await _userManager.FindByEmailAsync(vm.UserEmail);

      if (vm.Delete)
        await _userManager.RemoveFromRoleAsync(user, vm.Role);
      else
        await _userManager.AddToRoleAsync(user, vm.Role);

      return RedirectToAction("Index");
    }

  }
}
