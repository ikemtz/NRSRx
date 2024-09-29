using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Core.Unigration.Fakes
{
  /// <summary>
  /// A fake HTTP message handler for testing purposes.
  /// </summary>
  public class FakeHttpMessageHandler : HttpMessageHandler
  {
    /// <summary>
    /// Gets or sets the logic to generate a response for a given request.
    /// </summary>
    public Func<HttpRequestMessage, HttpResponseMessage> ResponseLogic { get; set; }

    /// <summary>
    /// Sends an HTTP request and returns an HTTP response.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <returns>The HTTP response message.</returns>
    public virtual HttpResponseMessage Send(HttpRequestMessage request) => this.ResponseLogic(request);

    /// <summary>
    /// Sends an HTTP request as an asynchronous operation.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      return Task.FromResult(Send(request));
    }

    /// <summary>
    /// Creates an HTTP response message with JSON content.
    /// </summary>
    /// <param name="responseObject">The object to serialize as JSON.</param>
    /// <returns>The HTTP response message with JSON content.</returns>
    public static HttpResponseMessage HttpJsonResponseFactory(object responseObject)
    {
      var msg = new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new StringContent(JsonConvert.SerializeObject(responseObject, Constants.JsonSerializerSettings), Encoding.UTF8, "application/json")
      };
      return msg;
    }

    /// <summary>
    /// Creates a fake <see cref="HttpClient"/> with the specified response logic.
    /// </summary>
    /// <param name="responseLogic">The logic to generate a response for a given request.</param>
    /// <returns>A fake <see cref="HttpClient"/> instance.</returns>
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
