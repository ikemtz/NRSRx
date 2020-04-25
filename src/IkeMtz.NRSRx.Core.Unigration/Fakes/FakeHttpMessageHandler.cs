using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

    public static HttpResponseMessage HttpJsonResponseFactory(object responseObject)
    {
      var msg = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new StringContent(JsonConvert.SerializeObject(responseObject), Encoding.UTF8, "application/json")
      };
      return msg;
    }

    public static HttpClient FakeHttpClientFactory(Func<HttpRequestMessage, HttpResponseMessage> responseLogic)
    {
      using var msgHandler = new FakeHttpMessageHandler()
      {
        ResponseLogic = responseLogic,
      };
      return new HttpClient(msgHandler);
    }
  }
}
