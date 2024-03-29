using System;
using System.ComponentModel.DataAnnotations;

namespace IkeMtz.Samples.Models.V1
{
  // Generated by the SQL POCO Class Generator Script
  // Script is available at:
  // https://raw.githubusercontent.com/ikemtz/NRSRx/master/tools/sql-poco-class-generator.sql
  public partial class StudentCourse
  : IkeMtz.NRSRx.Core.Models.IIdentifiable, IkeMtz.NRSRx.Core.Models.IAuditable
  {
    [Required]
    public Guid Id { get; set; }
    [Required]
    public Guid StudentId { get; set; }
    [Required]
    public Guid SchoolId { get; set; }
    [Required]
    public Guid CourseId { get; set; }
    [Required]
    public Guid SchoolCourseId { get; set; }
    [Required]
    [MaxLength(50)]
    public string Semester { get; set; }
    [Required]
    public double FinalScore { get; set; }
    [Required]
    public int Year { get; set; }
    [Required]
    [MaxLength(250)]
    public string CreatedBy { get; set; }
    [Required]
    public DateTimeOffset CreatedOnUtc { get; set; }
    public int? UpdateCount { get; set; }
    [MaxLength(250)]
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }
    public virtual Student Student { get; set; }
    public virtual School School { get; set; }
    public virtual Course Course { get; set; }
    public virtual SchoolCourse SchoolCourse { get; set; }
  }
}
