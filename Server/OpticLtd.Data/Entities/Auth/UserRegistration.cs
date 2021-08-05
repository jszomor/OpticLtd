using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticLtd.Data.Entities.Auth
{
  public class UserRegistration
  {
    [Required]
    [MaxLength(100)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; }

    [Required]
    [MaxLength(100)]
    public string Password { get; set; }
  }
}
