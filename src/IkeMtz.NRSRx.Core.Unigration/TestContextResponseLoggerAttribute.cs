using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// A filter attribute that logs the server response to the <see cref="TestContext"/>.
  /// </summary>
  /// <remarks>
  /// Initializes a new instance of the <see cref="TestContextResponseLoggerAttribute"/> class.
  /// </remarks>
  /// <param name="testContext">The test context instance.</param>
  public sealed class TestContextResponseLoggerAttribute(TestContext testContext) : ResultFilterAttribute
  {
    private readonly TestContext _testContext = testContext;

    /// <summary>
    /// Called after the action result has been executed.
    /// </summary>
    /// <param name="context">The result executed context.</param>
    [ExcludeFromCodeCoverage]
    public override void OnResultExecuted(ResultExecutedContext context)
    {
      context = context ?? throw new ArgumentNullException(nameof(context));
      try
      {
        var result = JsonConvert.SerializeObject(context.Result, Constants.JsonSerializerSettings);
        _testContext.WriteLine($"Server Response: {result}");
      }
      catch (OutOfMemoryException exception)
      {
        _testContext.WriteLine($"OutOfMemoryException thrown attempting to serialize server response: {exception.Message}");
      }
      catch (JsonSerializationException exception)
      {
        _testContext.WriteLine($"Serialization Exception thrown attempting to serialize server response: {exception.Message}");
        _testContext.WriteLine($"Server Response: {context.Result}");
      }
      base.OnResultExecuted(context);
    }
  }
}
