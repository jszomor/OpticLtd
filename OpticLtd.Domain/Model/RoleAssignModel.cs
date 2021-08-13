﻿using Microsoft.AspNetCore.Identity;
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
    public RoleAssignModel()
    {
      Users = new List<IdentityUser>();
    }

    [Required]
    public string UserId { get; set; }

    [Required]
    public string RoleId { get; set; }

    public List<IdentityUser> Users { get; set; }
  }
}
