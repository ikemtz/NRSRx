namespace IkeMtz.NRSRx.Events
{
  public abstract class EventType
  {
    public abstract string EventSuffix { get; }
  }

  public class CreateEvent : EventType
  {
    public override string EventSuffix => "Create";
  }

  public class DeleteEvent : EventType
  {
    public override string EventSuffix => "Delete";
  }

  public class UpdateEvent : EventType
  {
    public override string EventSuffix => "Update";
  }

  public class SendEvent : EventType
  {
    public override string EventSuffix => "Send";
  }

  public class CreatedEvent : EventType
  {
    public override string EventSuffix => "Created";
  }

  public class DeletedEvent : EventType
  {
    public override string EventSuffix => "Deleted";
  }

  public class UpdatedEvent : EventType
  {
    public override string EventSuffix => "Updated";
  }

  public class SentEvent : EventType
  {
    public override string EventSuffix => "Sent";
  }

  public class ReceivedEvent : EventType
  {
    public override string EventSuffix => "Received";
  }
}
