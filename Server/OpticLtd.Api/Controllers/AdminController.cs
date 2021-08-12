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
    private readonly IdentityUserRole<string> _identityUserRole;
    private readonly ITokenServices _tokenServices;

    public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ITokenServices tokenServices)
    {
      _userManager = userManager;
      _roleManager = roleManager;
      _tokenServices = tokenServices;
    }

    #region User Management

    [HttpGet]
    [Route("GetUsers")]
    public async Task<IActionResult> GetUsers()
    {
      var users = await _userManager.GetUserAsync();

      if (!users.Any())
      {
        return BadRequestAuth("No user found");
      }
      return Ok(users);
    }

    [HttpGet]
    [Route("GetUsersWithRoles")]
    public IActionResult GetUsersWithRoles()
    {
      var role = _roleManager.Roles.SingleOrDefault(m => m.Name == "role");
      var users = _userManager.GetUsersInRoleAsync("");
      var userRole = _identityUserRole.



      if (!users.Any())
      {
        return BadRequestAuth("No user found");
      }
      return Ok(users);
    }

    [HttpGet]
    [Route("GetUserByName")]
    public IActionResult GetUsers(string name)
    {
      var users = _userManager.FindByNameAsync(name));

      if (users.Result.UserName.StartsWith(name))
      {
        return Ok(users);
      }
      return BadRequestAuth("No user found");
    }

    [HttpPost]
    [Route("AddUser")]
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
          return Ok();
        }
        else
        {
          return BadRequestAuth("User creation failed");
        }
      }

      return BadRequestAuth("Invalid payload");
    }

    [HttpPost]
    [Route("EditUser")]
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
          return Ok("Perform succeeded.");
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
    [Route("DeleteUser")]
    public async Task<IActionResult> DeleteUser(string userId)
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

    private readonly string[] ValidRoleNames = new[] { "Admin", "Manager", "User" };


    [HttpGet]
    [Route("GetRoles")]
    public IActionResult GetRoles()
    {
      var roles = _roleManager.Roles.ToList();

      if (!roles.Any())
      {
        return BadRequestAuth("No roles found in database.");
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

      return BadRequestAuth("No roles found in database.");
    }

    [HttpPost]
    [Route("CreateRole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {

      if (ValidRoleNames.Contains(roleName))
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
          await _roleManager.UpdateAsync(role);
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
      return Ok("Perform succeeded.");
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
        return BadRequestAuth("Invalid role name.");
      }
      return Ok("Perform succeeded.");
    }
    #endregion

    #region AssignRoles

    [HttpPost]
    [Route("AddUserToRole")]
    public async Task<IActionResult> AddUserToRole(RoleAssignModel assign)
    {
      var user = await _userManager.FindByIdAsync(assign.UserId);
      if (user == null) return BadRequestAuth("User not found");

      var role = await _roleManager.FindByNameAsync(assign.RoleId);
      if (role == null) return BadRequestAuth("Role not found");

      var result = await _userManager.AddToRoleAsync(user, role.Name);

      if (result.Succeeded)
      {
        return Ok("Assing done.");
      }

      return BadRequestAuth("Something went wrong!");
    }





    #endregion
  }
}
