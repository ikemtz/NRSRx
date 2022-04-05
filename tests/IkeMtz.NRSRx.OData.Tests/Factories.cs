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
      var school = CreateIdentifiable(CreateAuditable<School>());
      school.Name = fullName[..6];
      school.FullName = fullName[..30];
      school.TenantId = fullName[..5];
      return school;
    }

    public static Student StudentFactory()
    {
      var student = CreateIdentifiable(CreateAuditable<Student>());
      student.FirstName = Guid.NewGuid().ToString()[..6];
      student.LastName = Guid.NewGuid().ToString()[..6];
      student.BirthDate = DateTime.UtcNow;
      student.Email = $"{Guid.NewGuid().ToString()[4]}@x{Guid.NewGuid().ToString()[4]}.com";
      return student;
    }

    public static Course CourseFactory()
    {
      var course = CreateIdentifiable(CreateAuditable<Course>());
      course.Num = Guid.NewGuid().ToString()[..4];
      course.Title = Guid.NewGuid().ToString()[..6];
      course.Description = Guid.NewGuid().ToString()[..20];
      course.Department = Guid.NewGuid().ToString()[0..25];
      course.AvgScore = new Random().NextDouble();
      course.PassRate = new Random().Next();
      return course;
    }

    public static SchoolCourse SchoolCourseFactory(School school, Course course)
    {
      var schoolCourse = CreateIdentifiable(CreateAuditable<SchoolCourse>());

      schoolCourse.Course = course;
      schoolCourse.CourseId = course.Id;
      schoolCourse.School = school;
      schoolCourse.SchoolId = school.Id;
      schoolCourse.AvgScore = new Random().Next(0, 5);
      schoolCourse.PassRate = new Random().NextDouble();
      course.SchoolCourses.Add(schoolCourse);
      school.SchoolCourses.Add(schoolCourse);
      return schoolCourse;
    }

    internal static StudentCourse StudentCourseFactory(Student student, Course course, School school)
    {
      var studentCourse = CreateIdentifiable(CreateAuditable<StudentCourse>());

      studentCourse.Course = course;
      studentCourse.CourseId = course.Id;
      studentCourse.Student = student;
      studentCourse.StudentId = student.Id;
      studentCourse.School = school;
      studentCourse.SchoolId = school.Id;
      studentCourse.FinalScore = new Random().Next(0, 5);
      studentCourse.Semester = "Summer";
      studentCourse.Year = DateTime.UtcNow.Year;
      course.StudentCourses.Add(studentCourse);
      student.StudentCourses.Add(studentCourse);
      return studentCourse;
    }
  }
}
