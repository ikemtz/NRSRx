using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.OData.Tests;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.WebApi;
using IkeMtz.Samples.WebApi.Data;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

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
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken(x =>
        x.Remove(x.First(t => t.Type == JwtRegisteredClaimNames.Email))));

      var result = await client.PostAsJsonAsync($"api/v1/{nameof(Course)}s.json", item);
      Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task UpdateCourseCausesAuditInvalidUserExTest()
    {
      var originalCourse = Factories.CourseFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Courses.Add(originalCourse);
          });
        }));
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken(x =>
        x.Remove(x.First(t => t.Type == JwtRegisteredClaimNames.Email))));

      var updatedCourse = JsonConvert.DeserializeObject<Course>(JsonConvert.SerializeObject(originalCourse));
      updatedCourse.Num = Guid.NewGuid().ToString()[..6];

      var result = await client.PutAsJsonAsync($"api/v1/{nameof(Course)}s.json?id={updatedCourse.Id}", updatedCourse);
      Assert.AreEqual(HttpStatusCode.InternalServerError, result.StatusCode);
    }

  }
}
