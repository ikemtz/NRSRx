using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Events;
using IkeMtz.NRSRx.Events;
using IkeMtz.Samples.Events.Redis.Controllers.V1;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IkeMtz.Samples.Events.Redis.Tests.Unit
{
  [TestClass]
  [DoNotParallelize()]
  public partial class CoursesTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task CreateCoursesTest()
    {
      var mockPublisher = MockRedisStreamFactory<Course, CreatedEvent>.CreatePublisher();
      var course = Factories.CourseFactory();
      var result = await new CoursesController().Post(course, mockPublisher.Object)
        as OkResult;
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Course>(t => t.Id == course.Id)), Times.Once);
      Assert.AreEqual(200, result?.StatusCode);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task UpdateCoursesTest()
    {
      var mockPublisher = MockRedisStreamFactory<Course, UpdatedEvent>.CreatePublisher();
      var Course = Factories.CourseFactory();
      var result = await new CoursesController().Put(Course.Id, Course, mockPublisher.Object)
        as OkResult;
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Course>(t => t.Id == Course.Id)), Times.Once);
      Assert.AreEqual(200, result?.StatusCode);
    }

    [TestMethod]
    [TestCategory(TestCategories.Unit)]
    public async Task DeleteCoursesTest()
    {
      var mockPublisher = MockRedisStreamFactory<Course, DeletedEvent>.CreatePublisher();
      var Course = Factories.CourseFactory();
      var result = await new CoursesController().Delete(Course.Id, mockPublisher.Object)
        as OkResult;
      mockPublisher.Verify(t => t.PublishAsync(It.Is<Course>(t => t.Id == Course.Id)), Times.Once);
      Assert.AreEqual(200, result?.StatusCode);
    }
  }
}
