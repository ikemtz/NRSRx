using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Http;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Tests;
using IkeMtz.Samples.WebApi;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace IkeMtz.NRSRx.WebApi.Tests
{
  [TestClass]
  public class AuditableSecurityTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task SaveCourseCausesAuditInvalidUserExTest()
    {
      var item = Factories.CourseFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken(x =>
        x.Remove(x.First(t => t.Type == JwtRegisteredClaimNames.Email))));

      var result = await client.PostAsJsonAsync($"api/v1/{nameof(Course)}s.json", item);
      Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task UpdateCourseCausesAuditInvalidUserExTest()
    {
      var originalCourse = Factories.CourseFactory();
      originalCourse.CreatedBy = "xyz";
      originalCourse.CreatedOnUtc = DateTime.UtcNow.AddMonths(-500);
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Courses.Add(originalCourse);
          });
        }));
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken(x =>
        x.Remove(x.First(t => t.Type == JwtRegisteredClaimNames.Email))));

      var updatedCourse = JsonClone(originalCourse);
      updatedCourse.Num = Guid.NewGuid().ToString()[..6];

      var result = await client.PutAsJsonAsync($"api/v1/{nameof(Course)}s.json?id={updatedCourse.Id}", updatedCourse);
      Assert.AreEqual(HttpStatusCode.OK, result.StatusCode);
    }
  }
}
