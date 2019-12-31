using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public class TestContextResponseLoggerAttribute : ResultFilterAttribute
  {
    private readonly TestContext _testContext;
    public TestContextResponseLoggerAttribute(TestContext testContext)
    {
      _testContext = testContext;
    }
    public override void OnResultExecuted(ResultExecutedContext context)
    {
      var result = JsonConvert.SerializeObject(context.Result, Constants.JsonSerializerSettings);
      _testContext.WriteLine($"Server Response: {result}");
      base.OnResultExecuted(context);
    }
  }
}
