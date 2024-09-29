using System.Text;
using IkeMtz.NRSRx.Core;
using Newtonsoft.Json;

namespace IkeMtz.NRSRx.Events.Abstraction
{
  /// <summary>
  /// Provides methods for encoding and decoding messages to and from JSON.
  /// </summary>
  public static class MessageCoder
  {
    /// <summary>
    /// Encodes an entity to a JSON byte array.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="entity">The entity to encode.</param>
    /// <returns>A byte array representing the JSON-encoded entity.</returns>
    public static byte[] JsonEncode<TEntity>(TEntity entity)
    {
      var json = JsonConvert.SerializeObject(entity, Constants.JsonSerializerSettings);
      return Encoding.UTF8.GetBytes(json);
    }

    /// <summary>
    /// Decodes a JSON byte array to an entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="buffer">The byte array to decode.</param>
    /// <returns>The decoded entity.</returns>
    public static TEntity JsonDecode<TEntity>(byte[] buffer)
    {
      var json = Encoding.UTF8.GetString(buffer);
      return JsonConvert.DeserializeObject<TEntity>(json, Constants.JsonSerializerSettings);
    }
  }
}
