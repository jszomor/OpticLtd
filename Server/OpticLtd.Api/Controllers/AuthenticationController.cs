using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OpticLtd.Api.Configuration;
using OpticLtd.Api.Model;
using OpticLtd.Api.Model.DTOs.Request;
using OpticLtd.Api.Model.DTOs.Response;
using OpticLtd.Data;
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
    private readonly JwtConfig _jwtConfig;
    private readonly TokenValidationParameters _tokenValidationParams;
    private readonly AppDbContext _context;

    public AuthenticationController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor, TokenValidationParameters tokenValidationParams, AppDbContext context)
    {
      _userManager = userManager;
      _tokenValidationParams = tokenValidationParams;
      _context = context;
      _jwtConfig = optionsMonitor.CurrentValue;
    }

    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] UserRegistration user)
    {
      if(ModelState.IsValid)
      {
        var existingUser = await _userManager.FindByEmailAsync(user.Email);

        if(existingUser == null)
        {
          BadRequestAuth("Invalid login request");
        }

        var isCorrect = await _userManager.CheckPasswordAsync(existingUser, user.Password);

        if(!isCorrect)
        {
          BadRequestAuth("Invalid login request");
        }

        var jwtToken = await GenerateJwtToken(existingUser);

        return Ok(jwtToken);
      }
      return BadRequestAuth("Invalid payload");
    }

    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequest tokenRequest)
    {
      if(ModelState.IsValid)
      {
        var result = await VerifyAndGenerateToken(tokenRequest);
        if(result == null)
        {
          BadRequestAuth("Invalid tokens");
        }

        return Ok(result);
      }

      return BadRequestAuth("Invalid payload");
    }

    private async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest)
    {
      var jwtTokenHandler = new JwtSecurityTokenHandler();
      try
      {
        // Validation 1 - Validation JWT token format
        var tokenVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams,  out var validatedToken);

        // Validation 2
        if(validatedToken is JwtSecurityToken securityToken)
        {
          bool result = securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

          if(!result)
          {
            return null;
          }
        }

        // Validation 3
        var utcExpiryDate = long.Parse(tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
        var expiryDate = UnixTimeStampToDateTime(utcExpiryDate);
        var actualTime = DateTime.UtcNow.AddHours(2);
        if (expiryDate > actualTime) return AuthResultResponse("Token has not yet expired");

        // Validation 4 - validate existence of the token
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);
        if(storedToken == null) return AuthResultResponse("Token does not exist");

        // Validation 5
        if(storedToken.IsUsed) return AuthResultResponse("Token has been used");

        // Validation 6
        if(storedToken.IsRevoked) return AuthResultResponse("Token has been revoked");

        // Validation 7
        var jti = tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
        if (storedToken.JwtId != jti) return AuthResultResponse("Token does not match");

        // Updated current token
        storedToken.IsUsed = true;
        _context.RefreshTokens.Update(storedToken);
        await _context.SaveChangesAsync();

        // Generate a new token
        var dbUser = await _userManager.FindByIdAsync(storedToken.UserId);
        return await GenerateJwtToken(dbUser);
      }
      catch (Exception ex)
      {
        if (ex.Message.Contains("Lifetime validation failed. The token is expired."))
        {
          return AuthResultResponse("Token has expired please re-login");
        }
        else
        {
          return AuthResultResponse("Something went wrong");          
        }
      }
    }

    private AuthResult AuthResultResponse (string message)
    {
      return new AuthResult()
      {
        Success = false,
        Errors = new List<string>()
        {
          message
        }
      };
    }

    private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
      var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToLocalTime();
      return dateTimeVal;
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

    private async Task<AuthResult> GenerateJwtToken(IdentityUser user)
    {
      var jwtTokenHandler = new JwtSecurityTokenHandler();

      var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[]
        {
          new Claim("Id", user.Id),
          new Claim(JwtRegisteredClaimNames.Email, user.Email),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        }),
        Expires = DateTime.UtcNow.AddSeconds(30), //5-10 min
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      var token = jwtTokenHandler.CreateToken(tokenDescriptor);
      var jwtToken = jwtTokenHandler.WriteToken(token);

      var refreshToken = new Data.Entities.RefreshToken()
      {
        JwtId = token.Id,
        IsUsed = false,
        IsRevoked = false,
        UserId = user.Id,
        AddedDate = DateTime.UtcNow,
        ExpiryDate = DateTime.UtcNow.AddMonths(6),
        Token = string.Concat(RandomString(35) + Guid.NewGuid())
      };

      await _context.RefreshTokens.AddAsync(refreshToken);
      await _context.SaveChangesAsync();

      return new AuthResult()
      {
        Token = jwtToken,
        RefreshToken = refreshToken.Token,
        Success = true
      };
    }

    private string RandomString(int length)
    {
      var random = new Random();
      string availableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789+!%/=()_?,.";
      string generatedCode = new string(Enumerable.Repeat(availableChars, length).Select(x => x[random.Next(x.Length)]).ToArray());

      return generatedCode;
    }
  }
}
