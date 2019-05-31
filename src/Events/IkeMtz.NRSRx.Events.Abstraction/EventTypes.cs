namespace IkeMtz.NRSRx.Events
{
    public abstract class EventType
    {
        public abstract string EventSuffix { get; }
    }

    public class CreateEvent : EventType
    {
        public override string EventSuffix => "-create";
    }

    public class DeleteEvent : EventType
    {
        public override string EventSuffix => "-delete";
    }

    public class UpdateEvent : EventType
    {
        public override string EventSuffix => "-update";
    }

    public class SendEvent : EventType
    {
        public override string EventSuffix => "-send";
    }

    public class CreatedEvent : EventType
    {
        public override string EventSuffix => "-created";
    }

    public class DeletedEvent : EventType
    {
        public override string EventSuffix => "-deleted";
    }

    public class UpdatedEvent : EventType
    {
        public override string EventSuffix => "-updated";
    }

    public class SentEvent : EventType
    {
        public override string EventSuffix => "-sent";
    }

    public class ReceivedEvent : EventType
    {
        public override string EventSuffix => "-received";
    }
}
