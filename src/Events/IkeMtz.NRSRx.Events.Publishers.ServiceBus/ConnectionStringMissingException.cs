using System;

namespace IkeMtz.NRSRx.Events.Publishers.ServiceBus
{
  /// <summary>
  /// Exception thrown when a required connection string is missing.
  /// </summary>
  public class ConnectionStringMissingException : NullReferenceException
  {
    /// <summary>
    /// Gets or sets the name of the missing connection string.
    /// </summary>
    public string ConnectionStringName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringMissingException"/> class.
    /// </summary>
    public ConnectionStringMissingException() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringMissingException"/> class with a specified error message.
    /// </summary>
    /// <param name="connectionStringName">The name of the missing connection string.</param>
    public ConnectionStringMissingException(string connectionStringName)
      : base($"Connection string: {connectionStringName} value is missing")
    {
      ConnectionStringName = connectionStringName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionStringMissingException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="connectionStringName">The name of the missing connection string.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ConnectionStringMissingException(string connectionStringName, Exception innerException)
      : base($"Connection string: {connectionStringName} value is missing", innerException)
    {
      ConnectionStringName = connectionStringName;
    }
  }
}
