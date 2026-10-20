using System;
using System.Collections.Generic;
using IkeMtz.Samples.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace IkeMtz.NRSRx.WebApi.Tests.Controllers
{
  /// <summary>
  /// Controller for demonstrating issue 63857
  /// </summary>
  /// <seealso href="https://github.com/dotnet/aspnetcore/issues/63857"/>
  [Route("api/v{version:apiVersion}/[controller].{format}"), FormatFilter]
  [ApiVersion(VersionDefinitions.v1_0)]
  [ApiController]
  [Authorize]
  public class Issue63857Controller(ILogger<Issue63857Controller> logger) : ControllerBase
  {
    [HttpGet]
    [ProducesResponseType(Status200OK, Type = typeof(User))]
    public ActionResult<User> TestIssue()
    {
      logger.LogInformation("TestIssue called");
      var user = new User() { Name = "User" };
      return user;
    }
  }

  public class User
  {
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public User? Manager { get; set; }
    public ICollection<User> Subordinates { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<Car> Cars { get; set; } = [];
    public ICollection<Hourse> Hourses { get; set; } = [];
    public ICollection<Plan> Plans { get; set; } = [];
    public ICollection<Right> Rights { get; set; } = [];
    public ICollection<Salary> Salarys { get; set; } = [];
  }
  public class Car { public User? User { get; set; } }
  public class Hourse { public User? User { get; set; } }
  public class Order { public User? User { get; set; } }
  public class Plan { public User? User { get; set; } }
  public class Product { public User? User { get; set; } }
  public class Right { public User? User { get; set; } }
  public class Salary { public User? User { get; set; } }
}
