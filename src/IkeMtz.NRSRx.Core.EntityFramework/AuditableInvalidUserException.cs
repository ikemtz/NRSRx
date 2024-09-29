using System;
using System.Runtime.Serialization;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  /// <summary>
  /// Exception thrown when the current user does not have a valid username.
  /// </summary>
  [Serializable]
  public class AuditableInvalidUserException : Exception, ISerializable
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AuditableInvalidUserException"/> class.
    /// </summary>
    public AuditableInvalidUserException()
      : base("Current user does not have a valid username. Ensure that the Startup.JwtNameClaimMapping property is set correctly.")
    { }
  }
}
