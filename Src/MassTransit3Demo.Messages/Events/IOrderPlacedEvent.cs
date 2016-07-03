namespace MassTransit3Demo.Messages.Events
{
    public interface IOrderPlacedEvent : IEvent
    {
        decimal OrderAmount { get; set; }
    }
}
