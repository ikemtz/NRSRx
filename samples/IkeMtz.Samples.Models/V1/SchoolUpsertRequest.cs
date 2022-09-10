using System;
using System.ComponentModel.DataAnnotations;

namespace IkeMtz.Samples.Models.V1
{
  public class SchoolUpsertRequest
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