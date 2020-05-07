using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public sealed class TestContextResponseLoggerAttribute : ResultFilterAttribute
  {
    private readonly TestContext _testContext;
    public TestContextResponseLoggerAttribute(TestContext testContext)
    {
      _testContext = testContext;
    }
    public override void OnResultExecuted(ResultExecutedContext context)
    {
      context = context ?? throw new ArgumentNullException(nameof(context));
      try
      {
        var result = JsonConvert.SerializeObject(context.Result, Constants.JsonSerializerSettings);
        _testContext.WriteLine($"Server Response: {result}");
      }
      catch(JsonSerializationException exception)
      {
        _testContext.WriteLine($"Exception thrown attempting to serialize server response: {exception.Message}");
        _testContext.WriteLine($"Server Response: {context.Result}");
      }
      base.OnResultExecuted(context);
    }
  }
}
