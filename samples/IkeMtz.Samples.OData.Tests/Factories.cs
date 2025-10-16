using System;
using IkeMtz.Samples.Models.V1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static IkeMtz.NRSRx.Core.Unigration.TestDataFactory;

namespace IkeMtz.Samples.Tests
{
  [DoNotParallelize()]
  public static partial class Factories
  {
    public static School SchoolFactory()
    {
      var school = CreateIdentifiable<School>();
      school.Name = StringGenerator(6);
      school.FullName = StringGenerator(50);
      school.TenantId = StringGenerator(4);
      return school;
    }

    public static Student StudentFactory()
    {
      var student = CreateIdentifiable<Student>();
      student.FirstName = Guid.NewGuid().ToString()[..6];
      student.LastName = Guid.NewGuid().ToString()[..6];
      student.BirthDate = DateTime.UtcNow;
      student.Email = $"{Guid.NewGuid().ToString()[4]}@x{Guid.NewGuid().ToString()[4]}.com";
      return student;
    }

    public static Course CourseFactory()
    {
      var course = CreateIdentifiable<Course>();
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
      var schoolCourse = CreateIdentifiable<SchoolCourse>();
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

    internal static StudentCourse StudentCourseFactory(Student student, SchoolCourse schoolCourse)
    {
      var studentCourse = CreateIdentifiable<StudentCourse>();
      studentCourse.Course = schoolCourse.Course;
      studentCourse.CourseId = schoolCourse.CourseId;
      studentCourse.Student = student;
      studentCourse.StudentId = student.Id;
      studentCourse.School = schoolCourse.School;
      studentCourse.SchoolId = schoolCourse.School.Id;
      studentCourse.FinalScore = new Random().Next(0, 5);
      studentCourse.SchoolCourse = schoolCourse;
      studentCourse.SchoolCourseId = schoolCourse.Id;
      studentCourse.Semester = "Summer";
      studentCourse.Year = DateTime.UtcNow.Year;
      schoolCourse.Course.StudentCourses.Add(studentCourse);
      student.StudentCourses.Add(studentCourse);
      return studentCourse;
    }
  }
}
