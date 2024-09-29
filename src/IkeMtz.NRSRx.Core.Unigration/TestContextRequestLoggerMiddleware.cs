using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration
{
  /// <summary>
  /// Middleware for logging HTTP requests to the <see cref="TestContext"/>.
  /// </summary>
  /// <remarks>
  /// Initializes a new instance of the <see cref="TestContextRequestLoggerMiddleware"/> class.
  /// </remarks>
  /// <param name="next">The next middleware in the pipeline.</param>
  /// <param name="testContextInstance">The test context instance.</param>
  public class TestContextRequestLoggerMiddleware(RequestDelegate next, TestContext testContextInstance)
  {
    private readonly RequestDelegate _next = next;
    private readonly TestContext TestContext = testContextInstance;

    /// <summary>
    /// Invokes the middleware to log the HTTP request.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
      context = context ?? throw new ArgumentNullException(nameof(context));
      var request = context.Request;
      context.Request.EnableBuffering();
      TestContext.WriteLine($"** {request.Method} - {request.Path}{request.QueryString}: **");
      if (request.Body != null)
      {
        var reader = new StreamReader(context.Request.Body);
        context.Response.OnCompleted(() =>
        {
          reader.Dispose();
          return Task.CompletedTask;
        });
        var stringBuffer = await reader.ReadToEndAsync().ConfigureAwait(true);
        _ = context.Request.Body.Seek(0, SeekOrigin.Begin);
        TestContext.WriteLine($"Request Content: {stringBuffer}");
      }
      await _next.Invoke(context).ConfigureAwait(true);
    }
  }

  /// <summary>
  /// Extension methods for adding <see cref="TestContextRequestLoggerMiddleware"/> to the application pipeline.
  /// </summary>
  public static class TestContextRequestLoggerExtensions
  {
    /// <summary>
    /// Adds the <see cref="TestContextRequestLoggerMiddleware"/> to the application pipeline.
    /// </summary>
    /// <param name="builder">The application builder.</param>
    /// <param name="testContextInstance">The test context instance.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseTestContextRequestLogger(this IApplicationBuilder builder, TestContext? testContextInstance)
    {
      return builder.UseMiddleware<TestContextRequestLoggerMiddleware>(testContextInstance);
    }
  }
}
