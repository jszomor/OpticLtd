using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpticLtd.Domain.Model
{
  public class RoleAssignModel
  {
    [Required]
    public string UserId { get; set; }

    [Required]
    public string RoleId { get; set; }
  }
}
