using System.ComponentModel.DataAnnotations;

namespace OpticLtd.Domain.DTOs.Request
{
  public class UserRegistrationModel
  {
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public string PhoneNumber { get; set; }
  }
}
