using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Http;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Tests;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.Samples.WebApi.Tests.Integration
{
  [TestClass]
  public partial class CoursesTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task SaveCoursesTest()
    {
      var item = Factories.CourseFactory();
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationWebApiTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PostAsJsonAsync($"api/v1/{nameof(Course)}s.json", item);
      _ = resp.EnsureSuccessStatusCode();
      var httpCourse = await DeserializeResponseAsync<Course>(resp);
      Assert.IsNotNull(httpCourse);
      Assert.AreEqual("IntegrationTester@email.com", httpCourse.CreatedBy);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var dbCourse = await dbContext.Courses.FirstOrDefaultAsync(t => t.Id == item.Id);

      Assert.IsNotNull(dbCourse);
      Assert.AreEqual(httpCourse.CreatedOnUtc, dbCourse.CreatedOnUtc);
    }


    [TestMethod]
    [TestCategory(TestCategories.Integration)]
    public async Task UpdateCourseTest()
    {
      var originalCourse = Factories.CourseFactory();
      originalCourse.CreatedBy = "blah";
      originalCourse.CreatedOnUtc = DateTime.UtcNow;
      using var srv = new TestServer(TestWebHostBuilder<Startup, IntegrationWebApiTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Courses.Add(originalCourse);
          });
        }));
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var updatedCourse = JsonClone(originalCourse);
      updatedCourse.Num = Guid.NewGuid().ToString()[..6];

      var resp = await client.PutAsJsonAsync($"api/v1/{nameof(Course)}s.json?id={updatedCourse.Id}", updatedCourse);
      _ = resp.EnsureSuccessStatusCode();
      var httpUpdatedCourse = await DeserializeResponseAsync<Course>(resp);
      Assert.IsNotNull(httpUpdatedCourse);
      Assert.AreEqual("IntegrationTester@email.com", httpUpdatedCourse.UpdatedBy);
      Assert.AreEqual(updatedCourse.Num, httpUpdatedCourse.Num);
      Assert.IsNull(updatedCourse.UpdatedOnUtc);
      Assert.IsNotNull(httpUpdatedCourse.UpdatedOnUtc);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var updatedDbCourse = await dbContext.Courses.FirstOrDefaultAsync(t => t.Id == originalCourse.Id);

      Assert.IsNotNull(updatedDbCourse);
      Assert.IsNotNull(updatedDbCourse.UpdatedOnUtc);
      Assert.AreEqual(httpUpdatedCourse.UpdatedOnUtc, updatedDbCourse.UpdatedOnUtc);
    }

  }
}
