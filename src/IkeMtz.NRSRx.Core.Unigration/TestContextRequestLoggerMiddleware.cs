using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Core.Unigration
{
    public class TestContextRequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TestContext TestContext;

        public TestContextRequestLoggerMiddleware(RequestDelegate next, TestContext testContextInstance)
        {
            _next = next;
            TestContext = testContextInstance;
        }

        public async Task Invoke(HttpContext context)
        {
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
                string stringBuffer = await reader.ReadToEndAsync();
                context.Request.Body.Seek(0, SeekOrigin.Begin);
                TestContext.WriteLine($"Request Content: {stringBuffer}");
            }
            await _next.Invoke(context);
        }
    }

    public static class TestContextRequestLoggerExtensions
    {
        public static IApplicationBuilder UseTestContextRequestLogger(this IApplicationBuilder builder, TestContext testContextInstance)
        {
            return builder.UseMiddleware<TestContextRequestLoggerMiddleware>(testContextInstance);
        }
    }
}
