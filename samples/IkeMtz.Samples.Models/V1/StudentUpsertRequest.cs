using System;
using System.ComponentModel.DataAnnotations;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.Samples.Models.V1
{
  public class StudentUpsertRequest : IIdentifiable
  {

    [Required]
    public Guid Id { get; set; }
    [MaxLength(50)]
    public string? Title { get; set; }
    [Required]
    [MaxLength(250)]
    public string FirstName { get; set; }
    [Required]
    [MaxLength(250)]
    public string LastName { get; set; }
    [MaxLength(250)]
    public string? MiddleName { get; set; }
    [Required]
    public DateTime BirthDate { get; set; }
    [Required]
    public Gender Gender { get; set; }
    [Required]
    [MaxLength(250)]
    [EmailAddress]
    public string Email { get; set; }
    [MaxLength(15)]
    public string? Tel1 { get; set; }
    [MaxLength(15)]
    public string? Tel2 { get; set; }
    public int CourseCount { get; set; }
  }
}
