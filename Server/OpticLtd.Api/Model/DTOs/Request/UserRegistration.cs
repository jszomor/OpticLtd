using System.ComponentModel.DataAnnotations;

namespace OpticLtd.Api.Model.DTOs.Request
{
  public class UserRegistration
  {
    [Required]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
  }
}
