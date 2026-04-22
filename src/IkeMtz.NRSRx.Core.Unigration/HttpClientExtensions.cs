using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Core.Unigration.Http
{
  /// <summary>
  /// Provides extension methods for the <see cref="HttpClient"/> class to send JSON content.
  /// </summary>
  public static class HttpClientExtensions
  {
    /// <summary>
    /// Sends a POST request with JSON content to the specified URI.
    /// </summary>
    /// <typeparam name="T">The type of the content to send.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="requestUri">The URI to send the request to.</param>
    /// <param name="value">The content to send.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message.</returns>
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string requestUri, T value, CancellationToken? cancellationToken = null)
    {
      var json = JsonSerializer.Serialize(value, Constants.JsonSerializerOptions);
      var content = new StringContent(json, Encoding.UTF8, "application/json");
      return client.PostAsync(requestUri, content, cancellationToken ?? CancellationToken.None);
    }

    /// <summary>
    /// Sends a PUT request with JSON content to the specified URI.
    /// </summary>
    /// <typeparam name="T">The type of the content to send.</typeparam>
    /// <param name="client">The HTTP client.</param>
    /// <param name="requestUri">The URI to send the request to.</param>
    /// <param name="value">The content to send.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the HTTP response message.</returns>
    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, string requestUri, T value, CancellationToken? cancellationToken = null)
    {
      var json = JsonSerializer.Serialize(value, Constants.JsonSerializerOptions);
      var content = new StringContent(json, Encoding.UTF8, "application/json");
      return client.PutAsync(requestUri, content, cancellationToken ?? CancellationToken.None);
    }
  }
}
