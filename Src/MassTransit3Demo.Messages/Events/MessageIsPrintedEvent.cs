namespace MassTransit3Demo.Messages.Events
{
    public class MessageIsPrintedEvent : IEvent
    {
        public string Message { get; set; }

        public MessageIsPrintedEvent(string message)
        {
            Message = message;
        }
    }
}
