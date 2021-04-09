using System;
using IkeMtz.NRSRx.Core.Models;

namespace IkeMtz.NRSRx.Events.Publishers.Redis.Tests
{
  public class SampleMessage : IIdentifiable
  {
    public SampleMessage()
    {
      Id = Guid.NewGuid();
      Name = Guid.NewGuid().ToString();
    }
    public Guid Id { get; set; }
    public string Name { get; set; }
  }
}
