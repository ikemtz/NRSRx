using System;

namespace IkeMtz.NRSRx.Core.EntityFramework
{
  public class AuditableInvalidUserException : Exception
  {
    public AuditableInvalidUserException() : base("Current user does not have a valid username.") { }
  }
}
