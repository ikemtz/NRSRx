using System;
using System.Runtime.Serialization;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  [Serializable]
  public class AuditableInvalidUserException : Exception, ISerializable
  {
    public AuditableInvalidUserException() : base("Current user does not have a valid username.") { }
  }
}
