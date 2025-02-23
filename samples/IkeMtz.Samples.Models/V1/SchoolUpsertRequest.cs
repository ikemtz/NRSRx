using System;
using System.ComponentModel.DataAnnotations;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.Samples.Models.V1
{
  public class SchoolUpsertRequest : IIdentifiable
  {

    [Required]
    public Guid Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    [Required]
    [MaxLength(250)]
    public string FullName { get; set; }
    [Required]
    [MaxLength(5)]
    public string TenantId { get; set; }
  }
}
