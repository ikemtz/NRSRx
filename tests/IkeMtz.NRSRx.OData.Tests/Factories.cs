using System;
using IkeMtz.Samples.Models.V1;
using static IkeMtz.NRSRx.Core.Unigration.TestDataFactory;

namespace IkeMtz.NRSRx.OData.Tests
{
  public static partial class Factories
  {
    public static School SchoolFactory()
    {
      var school = IdentifiableFactory(AuditableFactory<School>());
      school.Name = Guid.NewGuid().ToString()[..6];
      school.TenantId = "NRSRX";
      return school;
    }

    public static Student StudentFactory()
    {
      var student = IdentifiableFactory(AuditableFactory<Student>());
      student.FirstName = Guid.NewGuid().ToString()[..6];
      student.LastName = Guid.NewGuid().ToString()[..6];
      student.BirthDate = DateTime.UtcNow;
      return student;
    }

    public static Course CourseFactory()
    {
      var course = IdentifiableFactory(AuditableFactory<Course>());
      course.Num = Guid.NewGuid().ToString()[..4];
      course.Title = Guid.NewGuid().ToString()[..6];
      course.Description = Guid.NewGuid().ToString()[..20];
      return course;
    }
  }
}
