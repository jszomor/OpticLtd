using Microsoft.AspNetCore.Identity;
using OpticLtd.Domain.Configuration;
using OpticLtd.Domain.DTOs.Request;
using System.Threading.Tasks;

namespace OpticLtd.BusinessLogic.Services
{
  public interface ITokenServices
  {
    Task<AuthResult> GenerateJwtToken(IdentityUser user);
    Task<AuthResult> VerifyAndGenerateToken(TokenRequestModel tokenRequest, UserManager<IdentityUser> _userManager);
  }
}