using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public static class HttpClientExtensions
  {
    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "We want to support URIs as strings")]
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
    {
      return client.PostAsync(requestUri, value, new JsonMediaTypeFormatter()
      {
        SerializerSettings = Constants.JsonSerializerSettings
      });
    }

    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "We want to support URIs as strings")]
    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
    {
      return client.PutAsync(requestUri, value, new JsonMediaTypeFormatter()
      {
        SerializerSettings = Constants.JsonSerializerSettings
      });
    }
  }
}
