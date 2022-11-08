using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.EntityFramework;
using IkeMtz.NRSRx.Core.Unigration;
using IkeMtz.NRSRx.Core.Unigration.Http;
using IkeMtz.Samples.Data;
using IkeMtz.Samples.Models.V1;
using IkeMtz.Samples.Tests;
using IkeMtz.Samples.WebApi;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.WebApi.Tests
{
  [TestClass]
  public class StudentTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetStudentTestAsync()
    {
      var item = Factories.StudentFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Students.Add(item);
          });
        }));
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken(new[] { new Claim("MyTestClaim", Guid.NewGuid().ToString()) }));
      //Get 
      var resp = await client.GetAsync($"api/v1/{nameof(Student)}s.json?id={item.Id}");
      var httpStudent = await DeserializeResponseAsync<Student>(resp);

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      Assert.IsNotNull(httpStudent);
      Assert.AreEqual(item.Title, httpStudent.Title);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task SaveStudentTest()
    {
      var item = Factories.StudentFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PostAsJsonAsync($"api/v1/{nameof(Student)}s.json", item);
      _ = resp.EnsureSuccessStatusCode();
      var httpStudent = await DeserializeResponseAsync<Student>(resp);
      Assert.IsNotNull(httpStudent);
      Assert.AreEqual(SystemUserProvider.SystemUserId, httpStudent.CreatedBy);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var dbStudents = await dbContext.Students.ToListAsync();

      Assert.AreEqual(1, dbStudents.Count);
      var dbStudent = dbStudents.FirstOrDefault();
      Assert.IsNotNull(dbStudent);
      Assert.AreEqual(httpStudent.CreatedOnUtc, dbStudent.CreatedOnUtc);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    [ExpectedException(typeof(JsonReaderException))]
    public async Task SaveStudentJsonReaderExceptionsTest()
    {
      var item = Factories.StudentFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>());
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.PostAsJsonAsync($"api/v1/{nameof(Student)}s.xml", item);
      _ = resp.EnsureSuccessStatusCode();
      _ = await DeserializeResponseAsync<Student>(resp);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task UpdateStudentTest()
    {
      var originalStudent = Factories.StudentFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Students.Add(originalStudent);
          });
        }));
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var updatedStudent = JsonClone(originalStudent);
      updatedStudent.Title = Guid.NewGuid().ToString()[..6];

      var resp = await client.PutAsJsonAsync($"api/v1/{nameof(Student)}s.json?id={updatedStudent.Id}", updatedStudent);
      _ = resp.EnsureSuccessStatusCode();
      var httpUpdatedStudent = await DeserializeResponseAsync<Student>(resp);
      Assert.IsNotNull(httpUpdatedStudent);
      Assert.AreEqual(SystemUserProvider.SystemUserId, httpUpdatedStudent.UpdatedBy);
      Assert.AreEqual(updatedStudent.Title, httpUpdatedStudent.Title);
      Assert.IsNull(updatedStudent.UpdatedOnUtc);
      Assert.IsNotNull(httpUpdatedStudent.UpdatedOnUtc);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var dbStudents = await dbContext.Students.ToListAsync();

      Assert.AreEqual(1, dbStudents.Count);
      var updatedDbStudent = dbStudents.FirstOrDefault();
      Assert.IsNotNull(updatedDbStudent);
      Assert.IsNotNull(updatedDbStudent.UpdatedOnUtc);
      Assert.AreEqual(httpUpdatedStudent.UpdatedOnUtc, updatedDbStudent.UpdatedOnUtc);
    }

    [TestMethod]
    [TestCategory("Unigration")]
    public async Task DeleteStudentTest()
    {
      var item = Factories.StudentFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationTestStartup>()
        .ConfigureTestServices(x =>
        {
          ExecuteOnContext<DatabaseContext>(x, db =>
          {
            _ = db.Students.Add(item);
          });
        }));
      var client = srv.CreateClient(TestContext);
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.DeleteAsync($"api/v1/{nameof(Student)}s.json?id={item.Id}");
      _ = resp.EnsureSuccessStatusCode();
      var httpUpdatedStudent = await DeserializeResponseAsync<Student>(resp);
      Assert.IsNotNull(httpUpdatedStudent);
      Assert.IsNull(httpUpdatedStudent.UpdatedBy);

      var dbContext = srv.GetDbContext<DatabaseContext>();
      var dbStudents = await dbContext.Students.ToListAsync();

      Assert.AreEqual(0, dbStudents.Count);
    }
  }
}
