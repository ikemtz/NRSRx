using System.Text;
using IkeMtz.NRSRx.Core;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Events.Abstraction
{
  public static class MessageCoder
  {
    public static byte[] JsonEncode<TEntity>(TEntity entity)
    {
      var json = JsonConvert.SerializeObject(entity, Constants.JsonSerializerSettings);
      return Encoding.UTF8.GetBytes(json);
    }

    public static TEntity JsonDecode<TEntity>(byte[] buffer)
    {
      var json = Encoding.UTF8.GetString(buffer);
      return JsonConvert.DeserializeObject<TEntity>(json, Constants.JsonSerializerSettings);
    }
  }
}
