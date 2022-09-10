using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IkeMtz.NRSRx.Core.Unigration.Logging
{
  public class HttpClientLoggingHandler : DelegatingHandler
  {
    public TestContext TestContext { get; }
    public HttpClientLoggingHandler(TestContext testContext, HttpMessageHandler testingHttpMessageHandler) : base(testingHttpMessageHandler)
    {
      TestContext = testContext;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      TestContext.WriteLine("Http Client Message Request:");
      TestContext.WriteLine($"  Request Uri: {request.RequestUri}");
      TestContext.WriteLine($"  Request Method: {request.Method}");
      TestContext.WriteLine($"  Request Version: {request.Version}");
      request.Headers
        .Where(t => t.Value.Any(a => !string.IsNullOrWhiteSpace(a)))
        .ToList()
        .ForEach(x =>
        {
          TestContext.WriteLine($"  Request Header: {x.Key} = {string.Join(";", x.Value)}");
        });
      if (request.Content != null)
      {
        TestContext.WriteLine($"  Request Content: {await request.Content.ReadAsStringAsync(cancellationToken)}");
      }
      else
      {
        TestContext.WriteLine($"  Request Content: << NO CONTENT >>");
      }

      var response = await base.SendAsync(request, cancellationToken);

      TestContext.WriteLine("Http Client Message Response:");
      TestContext.WriteLine($"  Response Status Code: {(int)response.StatusCode} {response.StatusCode}");
      TestContext.WriteLine($"  Response Status Version: {response.Version}");
      response.Headers
        .Where(t => t.Value.Any(a => !string.IsNullOrWhiteSpace(a)))
        .ToList()
        .ForEach(x =>
        {
          TestContext.WriteLine($"  Response Header: {x.Key} = {string.Join(";", x.Value)}");
        });

      if (response.Content != null)
      {
        TestContext.WriteLine($"  Response Content: {await response.Content.ReadAsStringAsync(cancellationToken)}");
      }
      else
      {
        TestContext.WriteLine($"  Response Content: << NO CONTENT >>");
      }
      return response;
    }
  }
}
