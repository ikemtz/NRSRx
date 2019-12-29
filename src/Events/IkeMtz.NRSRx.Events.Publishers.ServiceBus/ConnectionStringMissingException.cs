using System;

namespace IkeMtz.NRSRx.Events.Publishers.ServiceBus
{
  public class ConnectionStringMissingException : NullReferenceException
  {
    public string ConnectionStringName { get; set; }
    public ConnectionStringMissingException() { }
    public ConnectionStringMissingException(string connectionStringName) : base($"Connection string: ${connectionStringName} value is missing")
    {
      ConnectionStringName = connectionStringName;
    }

    public ConnectionStringMissingException(string connectionStringName, Exception innerException) : base($"Connection string: ${connectionStringName} value is missing", innerException)
    {
      ConnectionStringName = connectionStringName;
    } 
  }
}
