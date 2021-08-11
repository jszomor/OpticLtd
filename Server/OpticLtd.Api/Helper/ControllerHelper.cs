using Microsoft.AspNetCore.Mvc;
using OpticLtd.Domain.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpticLtd.Api.Helper
{
  public class ControllerHelper : ControllerBase
  {
    public BadRequestObjectResult BadRequestAuth(string message)
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
  }
}
