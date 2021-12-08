using System;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Jwt;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class TokenValidatorTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public async Task ValidateAADB2cToken()
    {
      var token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6Ilg1ZVhrNHh5b2pORnVtMWtsMll0djhkbE5QNC1jNTdkTzZRR1RWQndhTmsifQ.eyJleHAiOjE1ODU3NTA4OTksIm5iZiI6MTU4NTc0NzI5OSwidmVyIjoiMS4wIiwiaXNzIjoiaHR0cHM6Ly9tYXN0ZXJjb3JwYjJjLmIyY2xvZ2luLmNvbS8xMjA3OGM2Zi1mMTY5LTQ5ZWUtYjIzZC0zMTI5MmE4YmY5NDMvdjIuMC8iLCJzdWIiOiJkODIxZjIyMy0wZWJjLTQ3ZjQtYTQxOS05MDE3MWEwYjg2MWEiLCJhdWQiOiJmMjYwZjlmYS0yOGVkLTRlMDYtODI0YS01NDIwZDBjY2UzOGIiLCJub25jZSI6ImIzNTFjYTdjLTRmZDYtNGQ0MS05MjA2LTM0Y2ZjZDYxMmE1OCIsImlhdCI6MTU4NTc0NzI5OSwiYXV0aF90aW1lIjoxNTg1NzQ3Mjk5LCJpZHAiOiJodHRwczovL2xvZ2luLm1pY3Jvc29mdG9ubGluZS5jb20vMTEyMmQzZGYtODQwYy00M2JkLTkyNDUtMTc4ZmVlN2Q5M2MwL3YyLjAiLCJvaWQiOiJkODIxZjIyMy0wZWJjLTQ3ZjQtYTQxOS05MDE3MWEwYjg2MWEiLCJuYW1lIjoidW5rbm93biIsImVtYWlscyI6WyJpc2FhYy5tYXJ0aW5lekBtYXN0ZXJjb3JwLmNvbSJdLCJ0ZnAiOiJCMkNfMV9sZWdhY3kifQ.BFveB5TFg9TFhbyN-Oa7nBxpNaWb9aHvnUuNKRzenf4k_-3Ba52_miMf-spExoB8GCTroVRa4C246zOt-2Te9i_PyWB6aUPneyNmXffA3WLtsv3pS24Mx8uMFkNwdhsRj2jiuA4m84-aLA7ts1tJykcjSCa-SOYOKRsDN5oVQ6Qb-df-60CGTjyS2YEcfZzyUXduzddjcrFWej97hKkD9Z4WiYTGacuPzTHpBeGn_ywbSOaIWDbROe8JMXg5pdIRIk89TblTKQL_FDINZ_ENu1I8ZHtGP9-GttciZdLIOVanrVAy5bmnfqwL-o0bK98ETw7uV0BgbH1ZizI-mwI8ng";
      var tokenValidator = new TokenValidtor();

      await tokenValidator.InitAsync(
        "https://MasterCorpB2C.b2clogin.com/MasterCorpB2C.onmicrosoft.com/B2C_1_legacy/v2.0/.well-known/openid-configuration",
        "https://mastercorpb2c.b2clogin.com/12078c6f-f169-49ee-b23d-31292a8bf943/v2.0/",
        "f260f9fa-28ed-4e06-824a-5420d0cce38b");
      tokenValidator.TokenValidationParameters.ValidateLifetime = false;
      var result = tokenValidator.ValidateToken(token);
      Assert.IsTrue(result);

    }

    [TestMethod]
    [TestCategory("Unit")]
    public void ValidateEpochDateConverter_FromDouble()
    {
      var result = EpochDateConverter.FromDouble(1638996843);
      Assert.AreEqual(new DateTime(2021, 12, 8, 20, 54, 03, DateTimeKind.Utc), result);
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void ValidateEpochDateConverter_ToDouble()
    {
      var result = EpochDateConverter.ToDouble(new DateTime(2021, 12, 8, 20, 54, 03, DateTimeKind.Utc));
      Assert.AreEqual(1638996843, result);
    }
  }
}
