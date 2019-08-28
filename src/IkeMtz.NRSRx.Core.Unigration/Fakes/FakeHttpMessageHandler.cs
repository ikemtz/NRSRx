using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
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

    public HttpResponseMessage HttpJsonResponseFactory(object obj)
    {
      var msg = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json")
      };
      return msg;
    }
  }
}
