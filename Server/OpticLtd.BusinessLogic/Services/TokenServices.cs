using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpticLtd.Data;
using OpticLtd.Domain.Configuration;
using OpticLtd.Domain.DTOs.Request;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Services
{
  public class TokenServices
  {
    private readonly TokenValidationParameters _tokenValidationParams;
    private readonly AppDbContext _context;
    private readonly JwtConfig _jwtConfig;

    public TokenServices(JwtConfig jwtConfig, AppDbContext context, TokenValidationParameters tokenValidationParams)
    {
      _jwtConfig = jwtConfig;
      _context = context;
      _tokenValidationParams = tokenValidationParams;
    }


    public async Task<AuthResult> VerifyAndGenerateToken(TokenRequest tokenRequest, UserManager<IdentityUser> _userManager)
    {
      var jwtTokenHandler = new JwtSecurityTokenHandler();
      try
      {
        // Validation 1 - Validation JWT token format
        var tokenVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParams, out var validatedToken);

        // Validation 2
        if (validatedToken is JwtSecurityToken securityToken)
        {
          bool result = securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);

          if (!result)
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
        if (storedToken == null) return AuthResultResponse("Token does not exist");

        // Validation 5
        if (storedToken.IsUsed) return AuthResultResponse("Token has been used");

        // Validation 6
        if (storedToken.IsRevoked) return AuthResultResponse("Token has been revoked");

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
    public async Task<AuthResult> GenerateJwtToken(IdentityUser user)
    {
      var jwtTokenHandler = new JwtSecurityTokenHandler();

      var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[]
        {
          new Claim("Id", user.Id),
          new Claim(JwtRegisteredClaimNames.Email, user.Email),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          new Claim(ClaimTypes.Role, "Admin")
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

    private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
    {
      var dateTimeVal = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      dateTimeVal = dateTimeVal.AddSeconds(unixTimeStamp).ToLocalTime();
      return dateTimeVal;
    }

    private AuthResult AuthResultResponse(string message)
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
  }
}
