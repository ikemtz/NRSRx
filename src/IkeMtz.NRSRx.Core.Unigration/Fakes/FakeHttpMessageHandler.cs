using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Core.Unigration.Fakes
{
  public class FakeHttpMessageHandler : HttpMessageHandler
  {
    public Func<HttpRequestMessage, HttpResponseMessage> ResponseLogic { get; set; }
    public virtual HttpResponseMessage Send(HttpRequestMessage request) => this.ResponseLogic(request);

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      return Task.FromResult(Send(request));
    }
  }
}
