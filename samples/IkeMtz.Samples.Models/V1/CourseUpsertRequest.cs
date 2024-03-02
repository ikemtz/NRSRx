using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace IkeMtz.Samples.Models.V1
{
  public class CourseUpsertRequest
  {

    [Required]
    public Guid Id { get; set; }
    public CourseStatus Status { get; set; }
    [Required]
    [MaxLength(150)]
    public string Department { get; set; }
    [Required]
    [MaxLength(10)]
    public string Num { get; set; }
    [Required]
    [MaxLength(150)]
    public string Title { get; set; }
    [MaxLength(500)]
    public string? Description { get; set; }
    [DefaultValue(0)]
    public double? PassRate { get; set; }
  }
}
