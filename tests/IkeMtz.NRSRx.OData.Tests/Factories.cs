using System;
using IkeMtz.Samples.Models.V1;
using static IkeMtz.NRSRx.Core.Unigration.TestDataFactory;

namespace IkeMtz.NRSRx.OData.Tests
{
  public static partial class Factories
  {
    public static School SchoolFactory()
    {
      //TODO: Create a bettery string generator
      var fullName = Guid.NewGuid().ToString();
      var school = IdentifiableFactory(AuditableFactory<School>());
      school.Name = fullName[..6];
      school.FullName = fullName[..30];
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
      course.Department = Guid.NewGuid().ToString()[0..25];
      return course;
    }

    public static SchoolCourse SchoolCourseFactory(School school, Course course)
    {
      var schoolCourse = IdentifiableFactory(AuditableFactory<SchoolCourse>());

      schoolCourse.Course = course;
      schoolCourse.School = school;
      schoolCourse.AvgScore = new Random().Next(0, 5);
      schoolCourse.PassRate = new Random().NextDouble();
      return schoolCourse;
    }
  }
}
