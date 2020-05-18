using System;
using IkeMtz.NRSRx.Core.Unigration.WebApi.Unit;
using IkeMtz.NRSRx.Core.WebApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Tests
{
  [TestClass]
  public class ControllerBaseExtensionTests
  {
    [TestMethod]
    [TestCategory("Unit")]
    public void TestBuildNumberOutput()
    {
      var controller = new TestController();
      var result = controller.GetBuildNumber();
      Assert.IsTrue(result.StartsWith("1."));
    }

    [TestMethod]
    [TestCategory("Unit")]
    public void TestGetPingResult()
    {
      var controller = ControllerFactory<TestController>.Create();
      var result = controller.Get(new ApiVersion(1, 5));
      Assert.IsInstanceOfType(result, typeof(OkObjectResult));
      var pingResult = (result as OkObjectResult).Value as PingResult;
      Assert.AreEqual("NRSRx Test Controller", pingResult.Name);
    }
  }
}
