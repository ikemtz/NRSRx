using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace IkeMtz.NRSRx.Core.Unigration
{
  public static class HttpClientExtensions
  {
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
    {
      return client.PostAsync(requestUri, value, new JsonMediaTypeFormatter()
      {
        SerializerSettings = Constants.JsonSerializerSettings
      });
    }
    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
    {
      return client.PutAsync(requestUri, value, new JsonMediaTypeFormatter()
      {
        SerializerSettings = Constants.JsonSerializerSettings
      });
    }
  }
}
