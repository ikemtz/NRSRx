namespace IkeMtz.NRSRx.Events
{
  /// <summary>
  /// Abstract base class representing an event type.
  /// </summary>
  public abstract class EventType
  {
    /// <summary>
    /// Gets the suffix associated with the event type.
    /// </summary>
    public abstract string EventSuffix { get; }
  }

  /// <summary>
  /// Represents a create event.
  /// </summary>
  public class CreateEvent : EventType
  {
    /// <summary>
    /// Gets the suffix for create events.
    /// </summary>
    public override string EventSuffix => "Create";
  }

  /// <summary>
  /// Represents a delete event.
  /// </summary>
  public class DeleteEvent : EventType
  {
    /// <summary>
    /// Gets the suffix for delete events.
    /// </summary>
    public override string EventSuffix => "Delete";
  }

  /// <summary>
  /// Represents an update event.
  /// </summary>
  public class UpdateEvent : EventType
  {
    /// <summary>
    /// Gets the suffix for update events.
    /// </summary>
    public override string EventSuffix => "Update";
  }

  /// <summary>
  /// Represents a send event.
  /// </summary>
  public class SendEvent : EventType
  {
    /// <summary>
    /// Gets the suffix for send events.
    /// </summary>
    public override string EventSuffix => "Send";
  }

  /// <summary>
  /// Represents a created event.
  /// </summary>
  public class CreatedEvent : EventType
  {
    /// <summary>
    /// Gets the suffix for created events.
    /// </summary>
    public override string EventSuffix => "Created";
  }

  /// <summary>
  /// Represents a deleted event.
  /// </summary>
  public class DeletedEvent : EventType
  {
    /// <summary>
    /// Gets the suffix for deleted events.
    /// </summary>
    public override string EventSuffix => "Deleted";
  }

  /// <summary>
  /// Represents an updated event.
  /// </summary>
  public class UpdatedEvent : EventType
  {
    /// <summary>
    /// Gets the suffix for updated events.
    /// </summary>
    public override string EventSuffix => "Updated";
  }

  /// <summary>
  /// Represents a sent event.
  /// </summary>
  public class SentEvent : EventType
  {
    /// <summary>
    /// Gets the suffix for sent events.
    /// </summary>
    public override string EventSuffix => "Sent";
  }

  /// <summary>
  /// Represents a received event.
  /// </summary>
  public class ReceivedEvent : EventType
  {
    /// <summary>
    /// Gets the suffix for received events.
    /// </summary>
    public override string EventSuffix => "Received";
  }
}
