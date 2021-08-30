using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OpticLtd.Domain.DTOs.Request
{
  public class TokenRequestModel
  {
    [Required]
    public string Token { get; set; }
    
    [Required]
    public string RefreshToken { get; set; }
  }
}
