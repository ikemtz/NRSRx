using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.Samples.Models.V1
{
  // Generated by the SQL POCO Class Generator Script
  // Script is available at:
  // https://raw.githubusercontent.com/ikemtz/NRSRx/master/tools/sql-poco-class-generator.sql

  public partial class Student
  : StudentUpsertRequest, IkeMtz.NRSRx.Core.Models.IIdentifiable, IkeMtz.NRSRx.Core.Models.IAuditable, ICalculateable
  {
    public Student()
    {
      StudentCourses = new HashSet<StudentCourse>();
      StudentSchools = new HashSet<StudentSchool>();
    }

    [Required]
    [MaxLength(250)]
    public string CreatedBy { get; set; }
    [Required]
    public DateTimeOffset CreatedOnUtc { get; set; }
    [MaxLength(250)]
    public string? UpdatedBy { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }
    public int? UpdateCount { get; set; }
    public virtual ICollection<StudentCourse> StudentCourses { get; }
    public virtual ICollection<StudentSchool> StudentSchools { get; }

    public void CalculateValues()
    {
      CourseCount = (StudentCourses != null) ? CourseCount : StudentCourses?.Count ?? 0;
    }
  }
}
