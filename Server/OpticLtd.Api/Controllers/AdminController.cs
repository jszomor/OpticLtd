using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpticLtd.Api.Helper;
using OpticLtd.BusinessLogic.Services;
using OpticLtd.Domain.DTOs.Request;
using OpticLtd.Domain.DTOs.Response;
using OpticLtd.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OpticLtd.Api.Controllers
{
  //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
  [ApiController]
  [Route("api/[controller]")]
  public class AdminController : ControllerHelper
  {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;


    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
      _userManager = userManager;
      _roleManager = roleManager;
    }

    #region User Management

    [HttpGet]
    [Route("GetUsers")]
    public IActionResult GetUsers()
    {
      var users = _userManager.Users.Select(x => new { x.Id, x.UserName, x.Email, x.PhoneNumber }).ToList();

      if (users == null)
      {
        return BadRequest("No user found");
      }
      return Ok(users);
    }

    [HttpGet]
    [Route("GetUsersByRole")]
    public async Task<IActionResult> GetUsersWithRoles([FromBody] string role)
    {
      var users = await _userManager.GetUsersInRoleAsync(role);

      if (!users.Any())
      {
        return BadRequest("No user found");
      }
      return Ok(users);
    }

    [HttpGet]
    [Route("GetUserByName")]
    public IActionResult GetUserByName(string name)
    {
      var users = _userManager.FindByNameAsync(name);

      if (users.Result.UserName.StartsWith(name))
      {
        return Ok(users);
      }
      return BadRequest("No user found");
    }

    [HttpPost]
    [Route("AddUser")]
    public async Task<IActionResult> AddUser(UserRegistrationModel user)
    {
      if (ModelState.IsValid)
      {
        var existingUser = await _userManager.FindByEmailAsync(user.Email);

        if (existingUser != null)
        {
          return BadRequest("Email already in use");
        }

        var newUser = new IdentityUser() { Email = user.Email, UserName = user.UserName, PhoneNumber = user.PhoneNumber };
        IdentityResult isCreated = await _userManager.CreateAsync(newUser, user.Password);

        if (isCreated.Succeeded)
        {
          return Ok();
        }
        else
        {
          return BadRequest("User creation failed");
        }
      }

      return BadRequest("Invalid payload");
    }

    [HttpPost]
    [Route("EditUser")]
    public async Task<IActionResult> EditUser(IdentityUser user)
    {
      if (!ModelState.IsValid) return BadRequest("Invalid payload");

      var existingUser = await _userManager.FindByIdAsync(user.Id);

      if (existingUser != null)
      {
        existingUser.Email = user.Email ?? existingUser.Email;
        existingUser.UserName = user.UserName ?? existingUser.UserName;
        existingUser.PhoneNumber = user.PhoneNumber ?? existingUser.PhoneNumber;

        var result = await _userManager.UpdateAsync(existingUser);
        if (result.Succeeded)
        {
          return Ok($"Edit succeeded. User: {existingUser.UserName}");
        }
        else
        {
          return BadRequest("Update operation failed");
        }
      }
      else
      {
        return BadRequest("Operation failed. Selected user does not exist.");
      }
    }

    [HttpPost]
    [Route("DeleteUser")]
    public async Task<IActionResult> DeleteUser([FromBody] string userId)
    {
      IdentityUser user = await _userManager.FindByIdAsync(userId);

      if (user != null)
      {
        IdentityResult result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
          return Ok("Perform succeeded.");
        }
        else
        {
          return BadRequest("Delete operation failed");
        }
      }
      else
      {
        return BadRequest("Operation failed. Selected user does not exist.");
      }
    }

    #endregion

    #region Role Management

    private readonly string[] ValidRoleNames = new[] { "Admin", "Manager", "Customer", "User" };


    [HttpGet]
    [Route("GetRoles")]
    public IActionResult GetRoles()
    {
      var roles = _roleManager.Roles.ToList();

      if (!roles.Any())
      {
        return BadRequest("No roles found in database.");
      }

      return Ok(roles);
    }

    [HttpGet]
    [Route("GetRoleByName")]
    public IActionResult GetRoleByName(string name)
    {
      var roles = _roleManager.FindByNameAsync(name);

      if (roles.Result.Name.StartsWith(name))
      {
        return Ok(roles.Result.Name);
      }

      return BadRequest("No roles found in database.");
    }

    [HttpPost]
    [Route("CreateRole")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {

      if (ValidRoleNames.Contains(roleName))
      {
        if (await _roleManager.RoleExistsAsync(roleName) == false)
        {
          await _roleManager.CreateAsync(new IdentityRole(roleName));
        }
        else
        {
          return BadRequest("Given role is already exist.");
        }
      }
      else
      {
        return BadRequest("Invalid role name.");
      }

      return Ok("Perform succeeded.");
    }

    [HttpPost]
    [Route("EditRole")]
    public async Task<IActionResult> EditRole(RoleChangeModel roleModel)
    {
      var role = await _roleManager.FindByNameAsync(roleModel.CurrentRole);
      if (role != null || ValidRoleNames.Contains(roleModel.NewRole))
      {
        if (await _roleManager.RoleExistsAsync(roleModel.NewRole) == false)
        {
          role.Name = roleModel.NewRole;
          role.NormalizedName = roleModel.NewRole.ToUpper();
          await _roleManager.UpdateAsync(role);
        }
        else
        {
          return BadRequest("Given role is already exist.");
        }
      }
      else
      {
        return BadRequest("Invalid role name.");
      }
      return Ok($"Role edit done. New role: {roleModel.NewRole}");
    }

    [HttpPost]
    [Route("DeleteRole")]
    public async Task<IActionResult> DeleteRole(RoleChangeModel roleModel)
    {
      var role = await _roleManager.FindByNameAsync(roleModel.CurrentRole);
      if (role != null)
      {
        await _roleManager.DeleteAsync(role);
      }
      else
      {
        return BadRequest("Invalid role name.");
      }
      return Ok($"Role deleted. {role.Name}");
    }
    #endregion

    #region AssignRoles

    [HttpPost]
    [Route("ConnectsUserToRoleById")]
    public async Task<IActionResult> ConnectsUserToRole(RoleAssignModel assign)
    {
      var user = await _userManager.FindByIdAsync(assign.UserId);
      if (user == null) return BadRequest("User not found");

      var role = await _roleManager.FindByIdAsync(assign.RoleId);
      if (role == null) return BadRequest("Role not found");

      var result = await _userManager.AddToRoleAsync(user, role.Name);

      if (result.Succeeded)
      {
        return Ok($"Assign done. User: {user} >> Role: {role}");
      }

      return BadRequest("Something went wrong!");
    }

    [HttpPost]
    [Route("DisconnectsUserFromRole")]
    public async Task<IActionResult> DisconnectsUserFromRole(RoleAssignModel assign)
    {
      var user = await _userManager.FindByIdAsync(assign.UserId);
      var role = await _roleManager.FindByIdAsync(assign.RoleId);

      var result = await _userManager.RemoveFromRoleAsync(user, role.Name);

      if (result.Succeeded)
      {
        return Ok($"Disconnects done. User: {user}, Role: {role}");
      }

      return BadRequest("Something went wrong!");
    }

    #endregion
  }
}
